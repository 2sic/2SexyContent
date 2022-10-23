﻿using ToSic.Eav.Logging;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// This is a dynamic object which contains multiple dynamic objects (Sources).
    /// It will try to find a value inside each source in the order the Sources are managed. 
    /// </summary>
    /// <remarks>New in 12.02</remarks>
    [PublicApi]
    public partial interface IDynamicStack: ISxcDynamicObject, ICanDebug
    {
        /// <summary>
        /// Get a source object which is used in the stack. Returned as a dynamic object. 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A dynamic object like a <see cref="IDynamicEntity"/> or similar. If not found, it will return a source which just-works, but doesn't have data. </returns>
        /// <remarks>
        /// Added in 2sxc 12.03
        /// </remarks>
        dynamic GetSource(string name);

        [PrivateApi]
        dynamic GetStack(params string[] names);

        /* IMPORTANT: KEEP THIS DEFINITION AND DOCS IN SYNC BETWEEN IDynamicEntity, IDynamicEntityGet and IDynamicStack */
        /// <summary>
        /// Get a value of the entity. Usually you will prefer the quick access like
        /// @content.FirstName - which will give you the same things as content.Get("FirstName").
        /// There are two cases to use this:
        /// - when you dynamically assemble the field name in your code, like when using App.Resources or similar use cases.
        /// - to access a field which has a conflicting name with this object, like Get("Parents")
        /// </summary>
        /// <param name="name"></param>
        /// <returns>An object which can be either a string, number, boolean or List&lt;IDynamicEntity&gt;, depending on the field type. Will return null if the field was not found. </returns>
        dynamic Get(string name);

        /* IMPORTANT: KEEP THIS DEFINITION AND DOCS IN SYNC BETWEEN IDynamicEntity, IDynamicEntityGet and IDynamicStack */
        /// <summary>
        /// Get a property using the string name. Only needed in special situations, as most cases can use the object.name directly
        /// </summary>
        /// <param name="name">the property name. </param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="language">Optional language code - like "de-ch" to prioritize that language</param>
        /// <param name="convertLinks">Optionally turn off if links like file:72 are looked up to a real link. Default is true.</param>
        /// <param name="debug">Set true to see more details in [Insights](xref:NetCode.Debug.Insights.Index) how the value was retrieved.</param>
        /// <returns>a dynamically typed result, can be string, bool, etc.</returns>
        dynamic Get(string name,
            // ReSharper disable once MethodOverloadWithOptionalParameter
            string noParamOrder = Eav.Parameters.Protector,
            string language = null,
            bool convertLinks = true,
            bool? debug = null
        );


        /* IMPORTANT: KEEP THIS DEFINITION AND DOCS IN SYNC BETWEEN IDynamicEntity, IDynamicEntityGet and IDynamicStack */
        ///// <summary>
        ///// Activate debugging, so that you'll see details in [Insights](xref:NetCode.Debug.Insights.Index) how the value was retrieved.
        ///// </summary>
        ///// <param name="debug"></param>
        //void SetDebug(bool debug);

    }
}