﻿using ToSic.Sxc.Adam;

namespace ToSic.Sxc.Data
{
    public partial interface ITypedItem
    {
        /// <summary>
        /// Get the ADAM (Automatic Digital Asset Manager) for this field.
        /// This is a folder which contains all the files and possibly folders which are uploaded on exactly this field.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The Folder object</returns>
        /// <remarks>Added in 16.02</remarks>
        IFolder Folder(string name);

        /// <summary>
        /// Get the file of the current field.
        /// There are a few scenarios:
        /// 
        /// 1. If it's a file/hyperlink field pointing to a file such as `file:27` it will retrieve the file itself.
        /// 1. If it's a file/hyperlink field pointing to a url such as `http://xyz` it will return `null`.
        /// 1. If it's a library field, it will just take the first file, as there is no value referencing a specific field
        /// 1. If it's any other field, will return `null`
        /// </summary>
        /// <param name="name"></param>
        /// <remarks>Added in 16.02</remarks>
        /// <returns></returns>
        IFile File(string name);
    }
}
