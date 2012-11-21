﻿// <copyright file="MouseClickHandler.cs" company="WebDriver Committers">
// Copyright 2007-2011 WebDriver committers
// Copyright 2007-2011 Google Inc.
// Portions copyright 2007 ThoughtWorks, Inc
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace OpenQA.Selenium.Remote.Server.CommandHandlers
{
    /// <summary>
    /// Provides the handler for the <see cref="DriverCommand.MouseClick"/> command.
    /// </summary>
    internal class MouseClickHandler : WebDriverCommandHandler
    {
        private bool leftMouseButton = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseClickHandler"/> class.
        /// </summary>
        /// <param name="locatorParameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to match a resource in the URL.</param>
        /// <param name="parameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to operate on the resource.</param>
        public MouseClickHandler(Dictionary<string, string> locatorParameters, Dictionary<string, object> parameters)
            : base(locatorParameters, parameters)
        {
            int button = (int)GetCommandParameter("button");
            this.leftMouseButton = button == 0;
        }

        /// <summary>
        /// Returns a string representing the description of this <see cref="CommandHandler"/>.
        /// </summary>
        /// <returns>A string representing the description of this <see cref="CommandHandler"/>.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[click mouse]");
        }

        /// <summary>
        /// Clicks at the last coordinates set for the driver.
        /// </summary>
        /// <returns>This command always returns <see langword="null"/>.</returns>
        public override object Execute()
        {
            IHasInputDevices hasInputDevicesDriver = this.Session.Driver as IHasInputDevices;
            IMouse mouse = hasInputDevicesDriver.Mouse;
            if (this.leftMouseButton)
            {
                mouse.Click(null);
            }
            else
            {
                mouse.ContextClick(null);
            }

            return null;
        }
    }
}