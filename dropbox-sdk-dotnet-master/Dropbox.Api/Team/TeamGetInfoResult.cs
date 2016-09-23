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
    /// <para>The team get info result object</para>
    /// </summary>
    public class TeamGetInfoResult
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<TeamGetInfoResult> Encoder = new TeamGetInfoResultEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<TeamGetInfoResult> Decoder = new TeamGetInfoResultDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="TeamGetInfoResult" />
        /// class.</para>
        /// </summary>
        /// <param name="name">The name of the team.</param>
        /// <param name="teamId">The ID of the team.</param>
        /// <param name="numLicensedUsers">The number of licenses available to the
        /// team.</param>
        /// <param name="numProvisionedUsers">The number of accounts that have been invited or
        /// are already active members of the team.</param>
        /// <param name="policies">The policies</param>
        public TeamGetInfoResult(string name,
                                 string teamId,
                                 uint numLicensedUsers,
                                 uint numProvisionedUsers,
                                 Dropbox.Api.TeamPolicies.TeamMemberPolicies policies)
        {
            if (name == null)
            {
                throw new sys.ArgumentNullException("name");
            }

            if (teamId == null)
            {
                throw new sys.ArgumentNullException("teamId");
            }

            if (policies == null)
            {
                throw new sys.ArgumentNullException("policies");
            }

            this.Name = name;
            this.TeamId = teamId;
            this.NumLicensedUsers = numLicensedUsers;
            this.NumProvisionedUsers = numProvisionedUsers;
            this.Policies = policies;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="TeamGetInfoResult" />
        /// class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        public TeamGetInfoResult()
        {
        }

        /// <summary>
        /// <para>The name of the team.</para>
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// <para>The ID of the team.</para>
        /// </summary>
        public string TeamId { get; protected set; }

        /// <summary>
        /// <para>The number of licenses available to the team.</para>
        /// </summary>
        public uint NumLicensedUsers { get; protected set; }

        /// <summary>
        /// <para>The number of accounts that have been invited or are already active members
        /// of the team.</para>
        /// </summary>
        public uint NumProvisionedUsers { get; protected set; }

        /// <summary>
        /// <para>Gets the policies of the team get info result</para>
        /// </summary>
        public Dropbox.Api.TeamPolicies.TeamMemberPolicies Policies { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="TeamGetInfoResult" />.</para>
        /// </summary>
        private class TeamGetInfoResultEncoder : enc.StructEncoder<TeamGetInfoResult>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(TeamGetInfoResult value, enc.IJsonWriter writer)
            {
                WriteProperty("name", value.Name, writer, enc.StringEncoder.Instance);
                WriteProperty("team_id", value.TeamId, writer, enc.StringEncoder.Instance);
                WriteProperty("num_licensed_users", value.NumLicensedUsers, writer, enc.UInt32Encoder.Instance);
                WriteProperty("num_provisioned_users", value.NumProvisionedUsers, writer, enc.UInt32Encoder.Instance);
                WriteProperty("policies", value.Policies, writer, Dropbox.Api.TeamPolicies.TeamMemberPolicies.Encoder);
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="TeamGetInfoResult" />.</para>
        /// </summary>
        private class TeamGetInfoResultDecoder : enc.StructDecoder<TeamGetInfoResult>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="TeamGetInfoResult" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override TeamGetInfoResult Create()
            {
                return new TeamGetInfoResult();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(TeamGetInfoResult value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "name":
                        value.Name = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "team_id":
                        value.TeamId = enc.StringDecoder.Instance.Decode(reader);
                        break;
                    case "num_licensed_users":
                        value.NumLicensedUsers = enc.UInt32Decoder.Instance.Decode(reader);
                        break;
                    case "num_provisioned_users":
                        value.NumProvisionedUsers = enc.UInt32Decoder.Instance.Decode(reader);
                        break;
                    case "policies":
                        value.Policies = Dropbox.Api.TeamPolicies.TeamMemberPolicies.Decoder.Decode(reader);
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
