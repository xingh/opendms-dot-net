﻿using System;

namespace Common.Work
{
    public class DownloadAssetJob : AssetJobBase
    {
        public DownloadAssetJob(IWorkRequestor requestor, ulong id, Data.FullAsset fullAsset,
            UpdateUIDelegate actUpdateUI, uint timeout, ErrorManager errorManager,
            FileSystem.IO fileSystem, Logger generalLogger, Logger networkLogger)
            : base(requestor, id, fullAsset, actUpdateUI, timeout, ProgressMethodType.Determinate,
            errorManager, fileSystem, generalLogger, networkLogger)
        {
        }

        public override JobBase Run()
        {
            _currentState = State.Active | State.Executing;

            try
            {
                StartTimeout();
            }
            catch (Exception e)
            {
                _errorManager.AddError(ErrorMessage.TimeoutFailedToStart(e, this, "DownloadAssetJob"));
                _currentState = State.Error;
                _requestor.WorkReport(_actUpdateUI, this, _fullAsset);
                return this;
            }

            if (!_fullAsset.MetaAsset.DownloadFromServer(this, _networkLogger))
            {
                _errorManager.AddError(ErrorMessage.JobRunFailed(null, this));
                _currentState = State.Error;
                _requestor.WorkReport(_actUpdateUI, this, _fullAsset);
                return this;
            }

            // Check for Error
            if (this.IsError || CheckForAbortAndUpdate())
            {
                _requestor.WorkReport(_actUpdateUI, this, _fullAsset);
                return this;
            }

            UpdateLastAction();

            _fullAsset.DataAsset.OnProgress += new Data.DataAsset.ProgressHandler(Run_DataAsset_OnProgress);

            if (!_fullAsset.DataAsset.DownloadFromServer(this, _fullAsset.MetaAsset, _networkLogger))
            {
                _errorManager.AddError(ErrorMessage.JobRunFailed(null, this));
                _currentState = State.Error;
                _requestor.WorkReport(_actUpdateUI, this, _fullAsset);
                return this;
            }

            UpdateLastAction();

            // Check for Error
            if (this.IsError || CheckForAbortAndUpdate())
            {
                _fullAsset.DataAsset.OnProgress -= Run_DataAsset_OnProgress;
                _requestor.WorkReport(_actUpdateUI, this, _fullAsset);
                return this;
            }

            _currentState = State.Active | State.Finished;
            _fullAsset.DataAsset.OnProgress -= Run_DataAsset_OnProgress;
            _requestor.WorkReport(_actUpdateUI, this, _fullAsset);
            return this;
        }

        void Run_DataAsset_OnProgress(Data.DataAsset sender, int percentComplete)
        {
            UpdateProgress((ulong)sender.BytesComplete, (ulong)sender.BytesTotal);

            // Don't update the UI if finished, the final update is handled by the Run() method.
            if (sender.BytesComplete != sender.BytesTotal)
                _requestor.WorkReport(_actUpdateUI, this, _fullAsset);
        }
    }
}
