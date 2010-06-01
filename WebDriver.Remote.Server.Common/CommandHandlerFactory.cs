﻿/* Copyright notice and license
Copyright 2007-2010 WebDriver committers
Copyright 2007-2010 Google Inc.
Portions copyright 2007 ThoughtWorks, Inc

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using OpenQA.Selenium.Remote.Server.CommandHandlers;

namespace OpenQA.Selenium.Remote.Server
{
    /// <summary>
    /// Creates instances of <see cref="CommandHandler"/> objects for the desired <see cref="DriverCommand"/>.
    /// </summary>
    public abstract class CommandHandlerFactory
    {
        #region Private members
        private static object lockObject = new object();
        private Dictionary<DriverCommand, ConstructorInfo> handlers = new Dictionary<DriverCommand, ConstructorInfo>();
        private ConstructorInfo commandNotImplementedHandlerConstructor; 
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandlerFactory"/> class.
        /// </summary>
        protected CommandHandlerFactory()
        {
            this.AddHandlers();
            this.commandNotImplementedHandlerConstructor = GetConstructor(typeof(CommandNotImplementedHandler));
        }
        #endregion

        protected Dictionary<DriverCommand, ConstructorInfo> Handlers
        {
            get { return handlers; }
        }

        #region Public methods
        /// <summary>
        /// Creates a new <see cref="CommandHandler"/> given the <see cref="DriverCommand"/>, locator parameters, and body parameters.
        /// </summary>
        /// <param name="commandName">The <see cref="DriverCommand"/> value to create a handler for.</param>
        /// <param name="locatorParameters">The parameters in the URL used to locate the resource.</param>
        /// <param name="parameters">The parameters in the body of the request used to act on the resource.</param>
        /// <returns>A <see cref="CommandHandler"/> capable of executing the desired command.</returns>
        public CommandHandler CreateHandler(DriverCommand commandName, Dictionary<string, string> locatorParameters, Dictionary<string, object> parameters)
        {
            CommandHandler handler = null;
            lock (lockObject)
            {
                // Use reflection. We could refactor to use the new operator, but
                // the perf hit for this shouldn't be too bad.
                ConstructorInfo constructor = this.GetHandlerConstructor(commandName);
                handler = constructor.Invoke(new object[] { locatorParameters, parameters }) as CommandHandler;
            }

            return handler;
        }

        /// <summary>
        /// Gets a value indicating whether the factory can create a handler for the given command.
        /// </summary>
        /// <param name="commandName">The <see cref="DriverCommand"/> value to create a handler for.</param>
        /// <returns><see langword="true"/> if the factory can create a handler for the command; otherwise <see langword="false"/>.</returns>
        public bool CanCreateHandler(DriverCommand commandName)
        {
            return this.handlers.ContainsKey(commandName);
        }
        #endregion

        /// <summary>
        /// Adds handler constructors to the dictionary of handler creators.
        /// </summary>
        protected abstract void AddHandlers();

        #region Private support methods
        protected static ConstructorInfo GetConstructor(Type handlerType)
        {
            Type[] parameterTypes = new Type[] { typeof(Dictionary<string, string>), typeof(Dictionary<string, object>) };
            ConstructorInfo constructor = handlerType.GetConstructor(parameterTypes);
            return constructor;
        }

        protected ConstructorInfo GetHandlerConstructor(DriverCommand commandName)
        {
            ConstructorInfo handlerConstructor = null;
            if (!this.CanCreateHandler(commandName))
            {
                handlerConstructor = this.commandNotImplementedHandlerConstructor;
            }
            else
            {
                handlerConstructor = this.handlers[commandName];
            }

            return handlerConstructor;
        }
        #endregion
    }
}