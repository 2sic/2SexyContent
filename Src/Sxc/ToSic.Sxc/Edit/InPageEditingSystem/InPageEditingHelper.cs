﻿using Newtonsoft.Json;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Edit.InPageEditingSystem
{
    public partial class InPageEditingHelper : HasLog<IInPageEditingSystem>, IInPageEditingSystem
    {
        public InPageEditingHelper() : base("Sxc.Edit") { }

        public void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            Log.LinkTo(codeRoot.Log);
            SetBlock(codeRoot.Block);
        }

        public IInPageEditingSystem SetBlock(IBlock block)
        {
            Block = block;
            Enabled = Block?.Context.UserMayEdit ?? false;
            if(Log.Parent == null && block != null) Log.LinkTo(block.Log);
            return this;
        }

        protected IBlock Block;

        #region Attribute-helper

        /// <inheritdoc/>
        public IHybridHtmlString Attribute(string name, string value)
            => !Enabled ? null : Build.Attribute(name, value);

        /// <inheritdoc/>
        public IHybridHtmlString Attribute(string name, object value)
            => !Enabled ? null : Build.Attribute(name, JsonConvert.SerializeObject(value));

        #endregion Attribute Helper

    }
}
