﻿using System.Web;

namespace ToSic.SexyContent
{
    public partial class View
    {
        public bool RenderNaked
            => _renderNaked ?? (_renderNaked = Request.QueryString["standalone"] == "true").Value;
        private bool? _renderNaked;

        private void SendStandalone(string renderedTemplate)
        {
            Response.Clear();
            Response.Write(renderedTemplate);
            Response.Flush();
            Response.SuppressContent = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

    }
}