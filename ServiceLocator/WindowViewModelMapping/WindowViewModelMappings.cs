using System;
using System.Collections.Generic;
using ServiceLocator.Annotations;

namespace ServiceLocator.WindowViewModelMapping
{
    /// <summary>
    /// Class describing the Window-ViewModel mappings used by the dialog service.
    /// </summary>
    public abstract class WindowViewModelMappings : IWindowViewModelMappings
    {
        protected IDictionary<Type, Type> Mappings { get; set; }

        /// <summary>
        /// Gets the window type based on registered ViewModel type.
        /// </summary>
        /// <param name="argViewModelType">The type of the ViewModel.</param>
        /// <returns>The window type based on registered ViewModel type.</returns>
        public virtual Type GetWindowTypeFromViewModelType(Type argViewModelType)
        {
            return Mappings[argViewModelType];
        }

        ///// <summary>
        ///// Initializes a new instance of the <see cref="WindowViewModelMappings"/> class.
        ///// </summary>
        //public WindowViewModelMappings()
        //{
        //    mappings = new Dictionary<Type, Type>
        //    {
        //        { typeof(MainViewModel), typeof(string) },
        //        { typeof(NewProjectViewModel), typeof(NewProjectView) },
        //        { typeof(EditProjectViewModel), typeof(NewProjectView) },
        //        { typeof(NewBacklogViewModel), typeof(NewBacklogView) },
        //        { typeof(EditBacklogViewModel), typeof(EditBacklogView) }

        //    };
        //}



    }

}
