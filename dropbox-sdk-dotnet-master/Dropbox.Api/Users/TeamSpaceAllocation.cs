// <auto-generated>
// Auto-generated by StoneAPI, do not modify.
// </auto-generated>

namespace Dropbox.Api.Users
{
    using sys = System;
    using col = System.Collections.Generic;
    using re = System.Text.RegularExpressions;

    using enc = Dropbox.Api.Stone;

    /// <summary>
    /// <para>The team space allocation object</para>
    /// </summary>
    public class TeamSpaceAllocation
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<TeamSpaceAllocation> Encoder = new TeamSpaceAllocationEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<TeamSpaceAllocation> Decoder = new TeamSpaceAllocationDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="TeamSpaceAllocation" />
        /// class.</para>
        /// </summary>
        /// <param name="used">The total space currently used by the user's team
        /// (bytes).</param>
        /// <param name="allocated">The total space allocated to the user's team
        /// (bytes).</param>
        public TeamSpaceAllocation(ulong used,
                                   ulong allocated)
        {
            this.Used = used;
            this.Allocated = allocated;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="TeamSpaceAllocation" />
        /// class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        public TeamSpaceAllocation()
        {
        }

        /// <summary>
        /// <para>The total space currently used by the user's team (bytes).</para>
        /// </summary>
        public ulong Used { get; protected set; }

        /// <summary>
        /// <para>The total space allocated to the user's team (bytes).</para>
        /// </summary>
        public ulong Allocated { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="TeamSpaceAllocation" />.</para>
        /// </summary>
        private class TeamSpaceAllocationEncoder : enc.StructEncoder<TeamSpaceAllocation>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(TeamSpaceAllocation value, enc.IJsonWriter writer)
            {
                WriteProperty("used", value.Used, writer, enc.UInt64Encoder.Instance);
                WriteProperty("allocated", value.Allocated, writer, enc.UInt64Encoder.Instance);
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="TeamSpaceAllocation" />.</para>
        /// </summary>
        private class TeamSpaceAllocationDecoder : enc.StructDecoder<TeamSpaceAllocation>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="TeamSpaceAllocation" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override TeamSpaceAllocation Create()
            {
                return new TeamSpaceAllocation();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(TeamSpaceAllocation value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "used":
                        value.Used = enc.UInt64Decoder.Instance.Decode(reader);
                        break;
                    case "allocated":
                        value.Allocated = enc.UInt64Decoder.Instance.Decode(reader);
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
