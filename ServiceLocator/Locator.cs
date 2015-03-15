using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ServiceLocator
{
    /// <summary>
    /// A very simple service locator.
    /// </summary>
    public static class Locator
    {
        private static Dictionary<Type, ServiceInfo> services = new Dictionary<Type, ServiceInfo>();

        /// <summary>
        /// Registers a service.
        /// </summary>
        public static void Register<TInterface, TImplemention>() where TImplemention : TInterface
        {
            Register<TInterface, TImplemention>(false);
        }


        /// <summary>
        /// Registers a service as a singleton.
        /// </summary>
        public static void RegisterSingleton<TInterface, TImplemention>() where TImplemention : TInterface
        {
            Register<TInterface, TImplemention>(true);
        }


        /// <summary>
        /// Resolves a service.
        /// </summary>
        public static TInterface Resolve<TInterface>()
        {
            try
            {
                return (TInterface) services[typeof (TInterface)].ServiceImplementation;
            }
            catch (KeyNotFoundException ex)
            {
                throw new ApplicationException(typeof(TInterface).Name + " has not been added to the Locator Service.", ex);
            }
        }

        /**/

        public static void Reset()
        {
            services.Clear();
        }

        public static void Register<TInterface, TImplemention>(TImplemention argImplementation) where TImplemention : TInterface
        {
            ServiceInfo localInfo = new ServiceInfo(typeof(TImplemention), true, argImplementation);

            services.Add(typeof(TInterface), localInfo);
        }

        /**/

        /// <summary>
        /// Registers a service.
        /// </summary>
        /// <param name="argIsSingleton">true if service is Singleton; otherwise false.</param>
        private static void Register<TInterface, TImplemention>(bool argIsSingleton) where TImplemention : TInterface
        {
            services.Add(typeof(TInterface), new ServiceInfo(typeof(TImplemention), argIsSingleton));
        }


        /// <summary>
        /// Class holding service information.
        /// </summary>
        class ServiceInfo
        {
            private Type _serviceImplementationType;
            private object _serviceImplementation;
            private bool isSingleton;


            /// <summary>
            /// Initializes a new instance of the <see cref="ServiceInfo"/> class.
            /// </summary>
            /// <param name="argServiceImplementationType">Type of the service implementation.</param>
            /// <param name="argIsSingleton">Whether the service is a Singleton.</param>
            public ServiceInfo(Type argServiceImplementationType, bool argIsSingleton)
            {
                _serviceImplementationType = argServiceImplementationType;
                isSingleton = argIsSingleton;
            }

            public ServiceInfo(Type argServiceImplementationType, bool argIsSingleton, object argConcreteClass)
                : this(argServiceImplementationType, argIsSingleton)
            {
                _serviceImplementation = argConcreteClass;
            }

            /// <summary>
            /// Gets the service implementation.
            /// </summary>
            public object ServiceImplementation
            {
                get
                {
                    if (isSingleton)
                    {
                        if (_serviceImplementation == null)
                        {
                            _serviceImplementation = CreateInstance(_serviceImplementationType);
                        }

                        return _serviceImplementation;
                    }

                    return CreateInstance(_serviceImplementationType);
                }
            }


            /// <summary>
            /// Creates an instance of a specific type.
            /// </summary>
            /// <param name="argType">The type of the instance to create.</param>
            private static object CreateInstance(Type argType)
            {
                if (services.ContainsKey(argType))
                {
                    return services[argType].ServiceImplementation;
                }

                ConstructorInfo ctor = argType.GetConstructors().First();

                var parameters =
                    from parameter in ctor.GetParameters()
                    select CreateInstance(parameter.ParameterType);

                return Activator.CreateInstance(argType, parameters.ToArray());
            }
        }
    
    }

}
