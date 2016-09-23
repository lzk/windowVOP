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
    /// <para>The device session object</para>
    /// </summary>
    /// <seealso cref="ActiveWebSession" />
    /// <seealso cref="DesktopClientSession" />
    /// <seealso cref="MobileClientSession" />
    public class DeviceSession
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<DeviceSession> Encoder = new DeviceSessionEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<DeviceSession> Decoder = new DeviceSessionDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="DeviceSession" /> class.</para>
        /// </summary>
        /// <param name="sessionId">The session id</param>
        /// <param name="ipAddress">The IP address of the last activity from this
        /// session</param>
        /// <param name="country">The country from which the last activity from this session
        /// was made</param>
        /// <param name="created">The time this session was created</param>
        /// <param name="updated">The time of the last activity from this session</param>
        public DeviceSession(string sessionId,
                             string ipAddress = null,
                             string country = null,
                             sys.DateTime? created = null,
                             sys.DateTime? updated = null)
        {
            if (sessionId == null)
            {
                throw new sys.ArgumentNullException("sessionId");
            }

            this.SessionId = sessionId;
            this.IpAddress = ipAddress;
            this.Country = country;
            this.Created = created;
            this.Updated = updated;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="DeviceSession" /> class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        public DeviceSession()
        {
        }

        /// <summary>
        /// <para>The session id</para>
        /// </summary>
        public string SessionId { get; protected set; }

        /// <summary>
        /// <para>The IP address of the last activity from this session</para>
        /// </summary>
        public string IpAddress { get; protected set; }

        /// <summary>
        /// <para>The country from which the last activity from this session was made</para>
        /// </summary>
        public string Country { get; protected set; }

        /// <summary>
        /// <para>The time this session was created</para>
        /// </summary>
        public sys.DateTime? Created { get; protected set; }

        /// <summary>
        /// <para>The time of the last activity from this session</para>
        /// </summary>
        public sys.DateTime? Updated { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="DeviceSession" />.</para>
        /// </summary>
        private class DeviceSessionEncoder : enc.StructEncoder<DeviceSession>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(DeviceSession value, enc.IJsonWriter writer)
            {
                WriteProperty("session_id", value.SessionId, writer, enc.StringEncoder.Instance);
                if (value.IpAddress != null)
                {
                    WriteProperty("ip_address", value.IpAddress, writer, enc.StringEncoder.Instance);
                }
                if (value.Country != null)
                {
                    WriteProperty("country", value.Country, writer, enc.StringEncoder.Instance);
                }
                if (value.Created != null)
                {
                    WriteProperty("created", value.Created.Value, writer, enc.DateTimeEncoder.Instance);
                }
                if (value.Updated != null)
                {
                    WriteProperty("updated", value.Updated.Value, writer, enc.DateTimeEncoder.Instance);
                }
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="DeviceSession" />.</para>
        /// </summary>
        private class DeviceSessionDecoder : enc.StructDecoder<DeviceSession>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="DeviceSession" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override DeviceSession Create()
            {
                return new DeviceSession();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(DeviceSession value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "session_id":
                        value.SessionId = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "ip_address":
                        value.IpAddress = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "country":
                        value.Country = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "created":
                        value.Created = enc.DateTimeDecoder.Instance.Decode(reader);
                        break;
                    case "updated":
                        value.Updated = enc.DateTimeDecoder.Instance.Decode(reader);
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
