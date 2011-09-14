﻿using System;
using OpenDMS.Networking.Http.Methods;

namespace OpenDMS.Storage.Providers.CouchDB.Commands
{
    public class PutAttachment : Base
    {
        public PutAttachment(IDatabase db, Model.Document document, string attachmentName, Model.Attachment attachment, System.IO.Stream stream)
            : base(new Put(UriBuilder.Build(db, document, attachmentName), attachment.ContentType, (ulong)attachment.AttachmentLength))
        {
            _stream = stream;
        }

        public override ReplyBase MakeReply(Response response)
        {
            try
            {
                return new PutAttachmentReply(response);
            }
            catch (Exception e)
            {
                Logger.Storage.Error("An exception occurred while creating the PutAttachmentReply.", e);
                throw;
            }
        }
    }
}