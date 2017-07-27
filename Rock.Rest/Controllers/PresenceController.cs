﻿// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using Rock.Data;
using Rock.Model;
using Rock.Rest.Filters;
using Rock.Web.Cache;

namespace Rock.Rest.Controllers
{
    /// <summary>
    /// Personal Devices REST API
    /// </summary>
    public partial class PresenceController : ApiControllerBase
    {
        /// <summary>
        /// Posts the specified presence list.
        /// </summary>
        /// <param name="presenceList">The presence list.</param>
        [Authenticate, Secured]
        [HttpPost]
        [System.Web.Http.Route( "api/Presence" )]
        public void Post( List<MACPresence> presenceList )
        {
            using ( var rockContext = new RockContext() )
            {
                var interactionChannel = new InteractionChannelService( rockContext ).Get( Rock.SystemGuid.InteractionChannel.WIFI_PRESENCE.AsGuid() );
                if ( interactionChannel != null )
                {
                    var interactionComponentIds = new Dictionary<string, int>();

                    var personalDeviceService = new PersonalDeviceService( rockContext );
                    var interactionService = new InteractionService( rockContext );
                    var interactionComponentService = new InteractionComponentService( rockContext );

                    var epochTime = new DateTime( 1970, 1, 1, 0, 0, 0, 0 ).ToLocalTime();

                    foreach ( var macPresence in presenceList )
                    {
                        var device = personalDeviceService.GetByMACAddress( macPresence.Mac );
                        if ( device == null )
                        {
                            device = new PersonalDevice();
                            device.MACAddress = macPresence.Mac;
                            personalDeviceService.Add( device );

                            rockContext.SaveChanges();
                        }

                        foreach ( var presence in macPresence.Presence )
                        {
                            if ( !interactionComponentIds.ContainsKey( presence.Space ))
                            {
                                var component = interactionComponentService
                                    .Queryable().AsNoTracking()
                                    .Where( c =>
                                        c.ChannelId == interactionChannel.Id &&
                                        c.Name == presence.Space )
                                    .FirstOrDefault();
                                if ( component == null )
                                {
                                    new InteractionComponent();
                                    component.ChannelId = interactionChannel.Id;
                                    component.Name = presence.Space;
                                    interactionComponentService.Add( component );
                                    rockContext.SaveChanges();
                                }

                                interactionComponentIds.Add( presence.Space, component.Id );
                            }

                            var interaction = new Interaction();
                            interaction.InteractionDateTime = epochTime.AddSeconds( presence.Arrive );
                            interaction.Operation = "Present";
                            interaction.InteractionComponentId = interactionComponentIds[presence.Space];
                            interaction.InteractionData = presence.ToJson();
                            interaction.PersonalDeviceId = device.Id;
                            interaction.PersonAliasId = device.PersonAliasId;

                            interactionService.Add( interaction );

                            rockContext.SaveChanges();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Presence Details
        /// </summary>
        public class Presence
        {
            /// <summary>
            /// Gets or sets the wifi space (zone) where device was present.
            /// </summary>
            /// <value>
            /// The space.
            /// </value>
            public string Space { get; set; }

            /// <summary>
            /// Gets or sets the arrival timestamp.
            /// </summary>
            /// <value>
            /// The arrive.
            /// </value>
            public int Arrive { get; set; }

            /// <summary>
            /// Gets or sets the departure timestamp.
            /// </summary>
            /// <value>
            /// The depart.
            /// </value>
            public int Depart { get; set; }

            /// <summary>
            /// Gets or sets the duration of presence.
            /// </summary>
            /// <value>
            /// The duration.
            /// </value>
            public int Duration { get; set; }
        }

        /// <summary>
        /// The presence for a device with specfic MAC address
        /// </summary>
        public class MACPresence
        {
            /// <summary>
            /// Gets or sets the MAC Address.
            /// </summary>
            /// <value>
            /// The mac.
            /// </value>
            public string Mac { get; set; }

            /// <summary>
            /// Gets or sets the presence information
            /// </summary>
            /// <value>
            /// The presence.
            /// </value>
            public List<Presence> Presence { get; set; }
        }
    }
}
