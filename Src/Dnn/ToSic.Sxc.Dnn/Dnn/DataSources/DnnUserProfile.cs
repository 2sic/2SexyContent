﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Entities.Users;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Builder;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Queries;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Dnn.DataSources
{
    /// <summary>
    /// Get DNN user profiles as <see cref="IEntity"/> objects of one or many users.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
	[VisualQuery(
        GlobalName = "ToSic.Sxc.Dnn.DataSources.DnnUserProfile, ToSic.Sxc.Dnn",
        Type = DataSourceType.Source, 
        DynamicOut = true,
	    ExpectsDataOfType = "|Config ToSic.SexyContent.DataSources.DnnUserProfileDataSource",
        PreviousNames = new []
        {
            "ToSic.SexyContent.Environment.Dnn7.DataSources.DnnUserProfileDataSource, ToSic.SexyContent"
        }
        )]
	public class DnnUserProfile : ExternalData
	{
		#region Configuration-properties
		private const string PropertiesKey = "Properties";
		private const string TitleFieldKey = "TitleField";
		private const string ContentTypeKey = "ContentType";
		private const string UserIdsKey = "UserIds";
		private const string UserIdsDefaultKeyToken = "[Settings:UserIds||disabled]";
		private const string PropertiesDefaultKeyToken = "[Settings:Properties||DisplayName,Email,FirstName,LastName,Username]";
		private const string EntityTitleDefaultKeyToken = "[Settings:TitleFieldName||DisplayName]";
		private const string ContentTypeDefaultToken = "[Settings:ContentTypeName||DnnUserInfo]";

        /// <summary>
        /// The user id list of users to retrieve, comma-separated
        /// </summary>
		public string UserIds
		{
			get => Configuration[UserIdsKey];
            set => Configuration[UserIdsKey] = value;
        }

        /// <summary>
        /// List of profile-properties to retrieve, comma-separated
        /// </summary>
		public string Properties
		{
			get => Configuration[PropertiesKey];
            set => Configuration[PropertiesKey] = value;
        }

		/// <summary>
		/// Gets or sets the Name of the ContentType to simulate
		/// </summary>
		public string ContentType
		{
			get => Configuration[ContentTypeKey];
            set => Configuration[ContentTypeKey] = value;
        }

		/// <summary>
		/// Gets or sets the Name of the Title Attribute of the DNN-UserInfo
		/// </summary>
		public string TitleField
		{
			get => Configuration[TitleFieldKey];
            set => Configuration[TitleFieldKey] = value;
        }

		#endregion

		public DnnUserProfile(ITenant tenant, IZoneMapper zoneMapper)
		{
			Out.Add(Constants.DefaultStreamName, new DataStream(this, Constants.DefaultStreamName, GetList));
			Configuration.Values.Add(UserIdsKey, UserIdsDefaultKeyToken);
			Configuration.Values.Add(PropertiesKey, PropertiesDefaultKeyToken);
			Configuration.Values.Add(ContentTypeKey, ContentTypeDefaultToken);
			Configuration.Values.Add(TitleFieldKey, EntityTitleDefaultKeyToken);

            _tenant = tenant;
            _zoneMapper = zoneMapper;
        }

        private readonly ITenant _tenant;
        private readonly IZoneMapper _zoneMapper;

		private List<IEntity> GetList()
		{
            Configuration.Parse();
			var realTenant = _tenant.Id != Eav.Constants.NullId ? _tenant : _zoneMapper.Init(Log).TenantOfApp(AppId);

			var properties = Properties.Split(',').Select(p => p.Trim()).ToArray();
            var portalId = realTenant.Id;

			// read all user Profiles
			ArrayList users;
			if (UserIds == "disabled")
				users = UserController.GetUsers(portalId);
			// read user Profiles of specified UserIds
			else
			{
				var userIds = UserIds.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
				users = new ArrayList();
				foreach (var user in userIds.Select(userId => UserController.GetUserById(portalId, userId)))
					users.Add(user);
			}

			// convert Profiles to Entities
			var result = new List<IEntity>();
			foreach (UserInfo user in users)
			{
				// add Profile-Properties
				var values = new Dictionary<string, object>();
				foreach (var property in properties)
				{
					string value;
					switch (property.ToLower())
					{
						case "displayname":
							value = user.DisplayName;
							break;
						case "email":
							value = user.Email;
							break;
						case "firstname":
							value = user.FirstName;
							break;
						case "lastname":
							value = user.LastName;
							break;
						case "username":
							value = user.Username;
							break;
						default:
							value = user.Profile.GetPropertyValue(property);
							break;
					}

					values.Add(property, value);
				}

				// create Entity and add to result
				var entity = new Eav.Data.Entity(Eav.Constants.TransientAppId, user.UserID, ContentTypeBuilder.Fake(ContentType) , values, TitleField);
				result.Add(entity);
			}

			return result;
		}
	}
}