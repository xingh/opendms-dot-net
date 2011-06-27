﻿
namespace OpenDMS.Storage.Providers.CouchDB.EngineMethods
{
    public class GetAllGroups : Base
    {
        private IDatabase _db = null;

        public GetAllGroups(EngineRequest request, IDatabase db)
            : base(request)
        {
            _db = db;
        }

        public override void Execute()
        {
            Commands.GetView cmd;

            try
            {
                if (_onActionChanged != null) _onActionChanged(_request, EngineActionType.Preparing, false);
            }
            catch (System.Exception e)
            {
                Logger.Storage.Error("An exception occurred while calling the OnActionChanged event.", e);
                throw;
            }

            try
            {
                cmd = new Commands.GetView(UriBuilder.Build(_db, "groups", "GetAll"));
            }
            catch (System.Exception e)
            {
                Logger.Storage.Error("An exception occurred while creating the GetView command.", e);
                throw;
            }
            
            // Run it straight back to the subscriber
            AttachSubscriberEvent(cmd, _onProgress);
            AttachSubscriberEvent(cmd, _onComplete);
            AttachSubscriberEvent(cmd, _onError);
            AttachSubscriberEvent(cmd, _onTimeout);

            try
            {
                if (_onActionChanged != null) _onActionChanged(_request, EngineActionType.GettingGroups, true);
            }
            catch (System.Exception e)
            {
                Logger.Storage.Error("An exception occurred while calling the OnActionChanged event.", e);
                throw;
            }

            try
            {
                cmd.Execute(_db.Server.Timeout, _db.Server.Timeout, _db.Server.BufferSize, _db.Server.BufferSize);
            }
            catch (System.Exception e)
            {
                Logger.Storage.Error("An exception occurred while executing the command.", e);
            }
        }
    }
}
