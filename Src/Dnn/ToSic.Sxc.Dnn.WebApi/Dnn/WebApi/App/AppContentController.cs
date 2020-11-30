﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.App;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Dnn.WebApi.App
{
    /// <inheritdoc />
    /// <summary>
    /// Direct access to app-content items, simple manipulations etc.
    /// Should check for security at each standard call - to see if the current user may do this
    /// Then we can reduce security access level to anonymous, because each method will do the security check
    /// </summary>
    [AllowAnonymous]
    public class AppContentController : SxcApiControllerBase
	{
        #region Constructor / DI
        protected override string HistoryLogName => "Api.ApCont";
        #endregion

        #region Get List / all of a certain content-type
        /// <summary>
        /// Get all Entities of specified Type
        /// </summary>
        [HttpGet]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public IEnumerable<Dictionary<string, object>> GetEntities(string contentType, string appPath = null) 
            => _build<AppContent>().Init(GetContext(), GetBlock(), Log).GetItems(contentType, appPath);

        #endregion

        #region GetOne by ID / GUID

	    [HttpGet]
	    [AllowAnonymous] // will check security internally, so assume no requirements
	    public Dictionary<string, object> GetOne(string contentType, int id, string appPath = null)
	        => GetAndSerializeOneAfterSecurityChecks(contentType,
                entityApi => entityApi.GetOrThrow(contentType, id), appPath);


        [HttpGet]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public Dictionary<string, object> GetOne(string contentType, Guid guid, string appPath = null)
            => GetAndSerializeOneAfterSecurityChecks(contentType,
                entityApi => entityApi.GetOrThrow(contentType, guid), appPath);
        


        /// <summary>
        /// Preprocess security / context, then get the item based on an passed in method, 
        /// ...then process/finish
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="getOne"></param>
        /// <param name="appPath"></param>
        /// <returns></returns>
        private Dictionary<string, object> GetAndSerializeOneAfterSecurityChecks(string contentType, Func<IEnumerable<IEntity>, IEntity> getOne, string appPath) 
            => _build<AppContent>().Init(GetContext(), GetBlock(), Log).GetOne(contentType, getOne, appPath);

        #endregion

        #region ContentBlock - retrieving data of the current instance as is (ATM DNN-specific)
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
	    public HttpResponseMessage GetContentBlockData()
        {
            Log.Add("get content block data");
            // Important note: we are NOT supporting url-view switch at the moment for this
            // reason is, that this kind of data-access is fairly special
            // and not recommended for future use cases, where we have the query etc.
            // IF you want to support View-switching in this, do a deep review w/2dm first!
            // - note that it's really not needed, as you can always use a query or something similar instead
            // - not also that if ever you do support view switching, you will need to ensure security checks

            var dataHandler = new AppContentJsonForInstance();
            var block = GetBlock();
            // must access engine to ensure pre-processing of data has happened, 
            // especially if the cshtml contains a override void CustomizeData()
            var engine = block.BlockBuilder.GetEngine(Purpose.PublishData);  
            engine.CustomizeData();

            var dataSource = block.Data;
            string json;
            if (dataSource.Publish.Enabled)
            {
                var publishedStreams = dataSource.Publish.Streams;
                var streamList = publishedStreams.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                json = dataHandler.GenerateJson(dataSource, streamList, block.Context.EditAllowed);
            }
            else
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden)
                    {ReasonPhrase = dataHandler.GeneratePleaseEnableDataError(GetContext().Module.Id)});
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;
        }
        #endregion

        #region Create

        [HttpPost]
        [AllowAnonymous] // will check security internally, so assume no requirements
        public Dictionary<string, object> CreateOrUpdate([FromUri] string contentType,
            [FromBody] Dictionary<string, object> newContentItem, [FromUri] int? id = null,
            [FromUri] string appPath = null)
            => _build<AppContent>().Init(GetContext(), GetBlock(),Log)
                .CreateOrUpdate(contentType, newContentItem, id, appPath);

        #endregion

        #region Delete

        [HttpDelete]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public void Delete(string contentType, int id, [FromUri] string appPath = null) 
            => _build<AppContent>().Init(GetContext(), GetBlock(), Log).Delete(contentType, id, appPath);

        [HttpDelete]
	    [AllowAnonymous]   // will check security internally, so assume no requirements
        public void Delete(string contentType, Guid guid, [FromUri] string appPath = null) 
            => _build<AppContent>().Init(GetContext(), GetBlock(), Log).Delete(contentType, guid, appPath);

        #endregion

    }
}
