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
    /// <para>The list member devices arg object</para>
    /// </summary>
    public class ListMemberDevicesArg
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<ListMemberDevicesArg> Encoder = new ListMemberDevicesArgEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<ListMemberDevicesArg> Decoder = new ListMemberDevicesArgDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ListMemberDevicesArg" />
        /// class.</para>
        /// </summary>
        /// <param name="teamMemberId">The team's member id</param>
        /// <param name="includeWebSessions">Whether to list web sessions of the team's
        /// member</param>
        /// <param name="includeDesktopClients">Whether to list linked desktop devices of the
        /// team's member</param>
        /// <param name="includeMobileClients">Whether to list linked mobile devices of the
        /// team's member</param>
        public ListMemberDevicesArg(string teamMemberId,
                                    bool includeWebSessions = true,
                                    bool includeDesktopClients = true,
                                    bool includeMobileClients = true)
        {
            if (teamMemberId == null)
            {
                throw new sys.ArgumentNullException("teamMemberId");
            }

            this.TeamMemberId = teamMemberId;
            this.IncludeWebSessions = includeWebSessions;
            this.IncludeDesktopClients = includeDesktopClients;
            this.IncludeMobileClients = includeMobileClients;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ListMemberDevicesArg" />
        /// class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        public ListMemberDevicesArg()
        {
            this.IncludeWebSessions = true;
            this.IncludeDesktopClients = true;
            this.IncludeMobileClients = true;
        }

        /// <summary>
        /// <para>The team's member id</para>
        /// </summary>
        public string TeamMemberId { get; protected set; }

        /// <summary>
        /// <para>Whether to list web sessions of the team's member</para>
        /// </summary>
        public bool IncludeWebSessions { get; protected set; }

        /// <summary>
        /// <para>Whether to list linked desktop devices of the team's member</para>
        /// </summary>
        public bool IncludeDesktopClients { get; protected set; }

        /// <summary>
        /// <para>Whether to list linked mobile devices of the team's member</para>
        /// </summary>
        public bool IncludeMobileClients { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="ListMemberDevicesArg" />.</para>
        /// </summary>
        private class ListMemberDevicesArgEncoder : enc.StructEncoder<ListMemberDevicesArg>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(ListMemberDevicesArg value, enc.IJsonWriter writer)
            {
                WriteProperty("team_member_id", value.TeamMemberId, writer, enc.StringEncoder.Instance);
                WriteProperty("include_web_sessions", value.IncludeWebSessions, writer, enc.BooleanEncoder.Instance);
                WriteProperty("include_desktop_clients", value.IncludeDesktopClients, writer, enc.BooleanEncoder.Instance);
                WriteProperty("include_mobile_clients", value.IncludeMobileClients, writer, enc.BooleanEncoder.Instance);
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="ListMemberDevicesArg" />.</para>
        /// </summary>
        private class ListMemberDevicesArgDecoder : enc.StructDecoder<ListMemberDevicesArg>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="ListMemberDevicesArg" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override ListMemberDevicesArg Create()
            {
                return new ListMemberDevicesArg();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(ListMemberDevicesArg value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "team_member_id":
                        value.TeamMemberId = enc.StringDecoder.Instance.Decode(reader);
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
