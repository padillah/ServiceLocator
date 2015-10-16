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
        private static readonly Dictionary<String, ServiceInfo> Services = new Dictionary<String, ServiceInfo>();

        /// <summary>
        /// Registers a service.
        /// </summary>
        public static void Register<TInterface, TImplemention>() where TImplemention : TInterface
        {
            PrivateRegister<TImplemention>(typeof(TInterface).Name);
        }

        public static void Register<TInterface, TImplemention>(String argReference) where TImplemention : TInterface
        {
            PrivateRegister<TImplemention>(argReference);
        }

        /// <summary>
        /// Registers a service as a singleton.
        /// </summary>
        public static void RegisterSingleton<TInterface, TImplemention>() where TImplemention : TInterface
        {
            PrivateRegister<TImplemention>(typeof(TInterface).Name, true);
        }

        public static void RegisterSingleton<TInterface, TImplemention>(TImplemention argImplementation)
            where TImplemention : TInterface
        {
            ServiceInfo localInfo = new ServiceInfo(typeof(TInterface), true, argImplementation);
            PrivateRegister<TImplemention>(typeof(TInterface).Name, true, localInfo);
        }

        public static void RegisterSingleton<TInterface, TImplemention>(string argReference, TImplemention argImplementation)
            where TImplemention : TInterface
        {
            ServiceInfo localInfo = new ServiceInfo(typeof(TInterface), true, argImplementation);
            PrivateRegister<TImplemention>(argReference, true, localInfo);
        }

        /// <summary>
        /// Resolves a service.
        /// </summary>
        public static TInterface Resolve<TInterface>()
        {
            try
            {
                //return (TInterface)services[typeof(TInterface)].ServiceImplementation;
                return (TInterface)Services[typeof(TInterface).Name].ServiceImplementation;
            }
            catch (KeyNotFoundException ex)
            {
                throw new ApplicationException(typeof(TInterface).Name + " has not been added to the Locator Service.", ex);
            }
        }

        public static TInterface Resolve<TInterface>(String argReference)
        {
            try
            {
                //return (TInterface)services[typeof(TInterface)].ServiceImplementation;
                return (TInterface)Services[argReference].ServiceImplementation;
            }
            catch (KeyNotFoundException ex)
            {
                throw new ApplicationException(argReference + " has not been added to the Locator Service.", ex);
            }
        }

        public static void Reset()
        {
            Services.Clear();
        }

        /***********************************************************************************************************************************************/

        private static void PrivateRegister<TImplementation>(String argReference, bool argIsSingleton = false,
            ServiceInfo argImplementation = null)
        {
            if (!argIsSingleton)
            {
                Services.Add(argReference, new ServiceInfo(typeof(TImplementation), false));
            }
            else
            {
                if (argImplementation == null)
                {
                    Services.Add(argReference, new ServiceInfo(typeof(TImplementation), true));
                }
                else
                {
                    Services.Add(argReference, argImplementation);
                }
            }
        }

        /***********************************************************************************************************************************************/

        /// <summary>
        /// Class holding service information.
        /// </summary>
        private class ServiceInfo
        {
            private readonly bool _isSingleton;
            private readonly Type _serviceImplementationType;
            private object _serviceImplementation;

            /// <summary>
            /// Initializes a new instance of the <see cref="ServiceInfo"/> class.
            /// </summary>
            /// <param name="argServiceImplementationType">Type of the service implementation.</param>
            /// <param name="argIsSingleton">Whether the service is a Singleton.</param>
            public ServiceInfo(Type argServiceImplementationType, bool argIsSingleton)
            {
                _serviceImplementationType = argServiceImplementationType;
                _isSingleton = argIsSingleton;
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
                    if (_isSingleton)
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
                ConstructorInfo ctor;

                if (Services.ContainsKey(argType.Name))
                {
                    return Services[argType.Name].ServiceImplementation;
                }

                var ctorList = argType.GetConstructors();

                if (ctorList.Any())
                {
                    ctor = ctorList.First();
                }
                else
                {
                    return null;
                }

                var parameters =
                    from parameter in ctor.GetParameters()
                    select CreateInstance(parameter.ParameterType);

                return Activator.CreateInstance(argType, parameters.ToArray());
            }
        }
    }
}