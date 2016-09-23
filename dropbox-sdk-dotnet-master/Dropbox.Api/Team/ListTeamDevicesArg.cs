// <auto-generated>
// Auto-generated by StoneAPI, do not modify.
// </auto-generated>

namespace Dropbox.Api.Team
{
    using sys = System;
    using col = System.Collections.Generic;
    using re = System.Text.RegularExpressions;

    using enc = Dropbox.Api.Stone;

    /// <summary>
    /// <para>The list team devices arg object</para>
    /// </summary>
    public class ListTeamDevicesArg
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<ListTeamDevicesArg> Encoder = new ListTeamDevicesArgEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<ListTeamDevicesArg> Decoder = new ListTeamDevicesArgDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ListTeamDevicesArg" />
        /// class.</para>
        /// </summary>
        /// <param name="cursor">At the first call to the <see
        /// cref="Dropbox.Api.Team.Routes.TeamRoutes.DevicesListTeamDevicesAsync" /> the cursor
        /// shouldn't be passed. Then, if the result of the call includes a cursor, the
        /// following requests should include the received cursors in order to receive the next
        /// sub list of team devices</param>
        /// <param name="includeWebSessions">Whether to list web sessions of the team
        /// members</param>
        /// <param name="includeDesktopClients">Whether to list desktop clients of the team
        /// members</param>
        /// <param name="includeMobileClients">Whether to list mobile clients of the team
        /// members</param>
        public ListTeamDevicesArg(string cursor = null,
                                  bool includeWebSessions = true,
                                  bool includeDesktopClients = true,
                                  bool includeMobileClients = true)
        {
            this.Cursor = cursor;
            this.IncludeWebSessions = includeWebSessions;
            this.IncludeDesktopClients = includeDesktopClients;
            this.IncludeMobileClients = includeMobileClients;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ListTeamDevicesArg" />
        /// class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        public ListTeamDevicesArg()
        {
            this.IncludeWebSessions = true;
            this.IncludeDesktopClients = true;
            this.IncludeMobileClients = true;
        }

        /// <summary>
        /// <para>At the first call to the <see
        /// cref="Dropbox.Api.Team.Routes.TeamRoutes.DevicesListTeamDevicesAsync" /> the cursor
        /// shouldn't be passed. Then, if the result of the call includes a cursor, the
        /// following requests should include the received cursors in order to receive the next
        /// sub list of team devices</para>
        /// </summary>
        public string Cursor { get; protected set; }

        /// <summary>
        /// <para>Whether to list web sessions of the team members</para>
        /// </summary>
        public bool IncludeWebSessions { get; protected set; }

        /// <summary>
        /// <para>Whether to list desktop clients of the team members</para>
        /// </summary>
        public bool IncludeDesktopClients { get; protected set; }

        /// <summary>
        /// <para>Whether to list mobile clients of the team members</para>
        /// </summary>
        public bool IncludeMobileClients { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="ListTeamDevicesArg" />.</para>
        /// </summary>
        private class ListTeamDevicesArgEncoder : enc.StructEncoder<ListTeamDevicesArg>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(ListTeamDevicesArg value, enc.IJsonWriter writer)
            {
                if (value.Cursor != null)
                {
                    WriteProperty("cursor", value.Cursor, writer, enc.StringEncoder.Instance);
                }
                WriteProperty("include_web_sessions", value.IncludeWebSessions, writer, enc.BooleanEncoder.Instance);
                WriteProperty("include_desktop_clients", value.IncludeDesktopClients, writer, enc.BooleanEncoder.Instance);
                WriteProperty("include_mobile_clients", value.IncludeMobileClients, writer, enc.BooleanEncoder.Instance);
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="ListTeamDevicesArg" />.</para>
        /// </summary>
        private class ListTeamDevicesArgDecoder : enc.StructDecoder<ListTeamDevicesArg>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="ListTeamDevicesArg" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override ListTeamDevicesArg Create()
            {
                return new ListTeamDevicesArg();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(ListTeamDevicesArg value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "cursor":
                        value.Cursor = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "include_web_sessions":
                        value.IncludeWebSessions = enc.BooleanDecoder.Instance.Decode(reader);
                        break;
                    case "include_desktop_clients":
                        value.IncludeDesktopClients = enc.BooleanDecoder.Instance.Decode(reader);
                        break;
                    case "include_mobile_clients":
                        value.IncludeMobileClients = enc.BooleanDecoder.Instance.Decode(reader);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
        }

        #endregion
    }
}
