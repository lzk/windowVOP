// <auto-generated>
// Auto-generated by StoneAPI, do not modify.
// </auto-generated>

namespace Dropbox.Api.TeamCommon
{
    using sys = System;
    using col = System.Collections.Generic;
    using re = System.Text.RegularExpressions;

    using enc = Dropbox.Api.Stone;

    /// <summary>
    /// <para>The group type determines how a group is created and managed.</para>
    /// </summary>
    public class GroupType
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<GroupType> Encoder = new GroupTypeEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<GroupType> Decoder = new GroupTypeDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="GroupType" /> class.</para>
        /// </summary>
        public GroupType()
        {
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is Team</para>
        /// </summary>
        public bool IsTeam
        {
            get
            {
                return this is Team;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a Team, or <c>null</c>.</para>
        /// </summary>
        public Team AsTeam
        {
            get
            {
                return this as Team;
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is UserManaged</para>
        /// </summary>
        public bool IsUserManaged
        {
            get
            {
                return this is UserManaged;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a UserManaged, or <c>null</c>.</para>
        /// </summary>
        public UserManaged AsUserManaged
        {
            get
            {
                return this as UserManaged;
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is Other</para>
        /// </summary>
        public bool IsOther
        {
            get
            {
                return this is Other;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a Other, or <c>null</c>.</para>
        /// </summary>
        public Other AsOther
        {
            get
            {
                return this as Other;
            }
        }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="GroupType" />.</para>
        /// </summary>
        private class GroupTypeEncoder : enc.StructEncoder<GroupType>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(GroupType value, enc.IJsonWriter writer)
            {
                if (value is Team)
                {
                    WriteProperty(".tag", "team", writer, enc.StringEncoder.Instance);
                    Team.Encoder.EncodeFields((Team)value, writer);
                    return;
                }
                if (value is UserManaged)
                {
                    WriteProperty(".tag", "user_managed", writer, enc.StringEncoder.Instance);
                    UserManaged.Encoder.EncodeFields((UserManaged)value, writer);
                    return;
                }
                if (value is Other)
                {
                    WriteProperty(".tag", "other", writer, enc.StringEncoder.Instance);
                    Other.Encoder.EncodeFields((Other)value, writer);
                    return;
                }
                throw new sys.InvalidOperationException();
            }
        }

        #endregion

        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="GroupType" />.</para>
        /// </summary>
        private class GroupTypeDecoder : enc.UnionDecoder<GroupType>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="GroupType" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override GroupType Create()
            {
                return new GroupType();
            }

            /// <summary>
            /// <para>Decode based on given tag.</para>
            /// </summary>
            /// <param name="tag">The tag.</param>
            /// <param name="reader">The json reader.</param>
            /// <returns>The decoded object.</returns>
            protected override GroupType Decode(string tag, enc.IJsonReader reader)
            {
                switch (tag)
                {
                    case "team":
                        return Team.Decoder.DecodeFields(reader);
                    case "user_managed":
                        return UserManaged.Decoder.DecodeFields(reader);
                    default:
                        return Other.Decoder.DecodeFields(reader);
                }
            }
        }

        #endregion

        /// <summary>
        /// <para>A group to which team members are automatically added. Applicable to <a
        /// href="https://www.dropbox.com/help/986">team folders</a> only.</para>
        /// </summary>
        public sealed class Team : GroupType
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<Team> Encoder = new TeamEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<Team> Decoder = new TeamDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="Team" /> class.</para>
            /// </summary>
            private Team()
            {
            }

            /// <summary>
            /// <para>A singleton instance of Team</para>
            /// </summary>
            public static readonly Team Instance = new Team();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="Team" />.</para>
            /// </summary>
            private class TeamEncoder : enc.StructEncoder<Team>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(Team value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="Team" />.</para>
            /// </summary>
            private class TeamDecoder : enc.StructDecoder<Team>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="Team" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override Team Create()
                {
                    return new Team();
                }

                /// <summary>
                /// <para>Decode fields without ensuring start and end object.</para>
                /// </summary>
                /// <param name="reader">The json reader.</param>
                /// <returns>The decoded object.</returns>
                public override Team DecodeFields(enc.IJsonReader reader)
                {
                    return Team.Instance;
                }
            }

            #endregion
        }

        /// <summary>
        /// <para>A group is created and managed by a user.</para>
        /// </summary>
        public sealed class UserManaged : GroupType
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<UserManaged> Encoder = new UserManagedEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<UserManaged> Decoder = new UserManagedDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="UserManaged" />
            /// class.</para>
            /// </summary>
            private UserManaged()
            {
            }

            /// <summary>
            /// <para>A singleton instance of UserManaged</para>
            /// </summary>
            public static readonly UserManaged Instance = new UserManaged();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="UserManaged" />.</para>
            /// </summary>
            private class UserManagedEncoder : enc.StructEncoder<UserManaged>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(UserManaged value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="UserManaged" />.</para>
            /// </summary>
            private class UserManagedDecoder : enc.StructDecoder<UserManaged>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="UserManaged" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override UserManaged Create()
                {
                    return new UserManaged();
                }

                /// <summary>
                /// <para>Decode fields without ensuring start and end object.</para>
                /// </summary>
                /// <param name="reader">The json reader.</param>
                /// <returns>The decoded object.</returns>
                public override UserManaged DecodeFields(enc.IJsonReader reader)
                {
                    return UserManaged.Instance;
                }
            }

            #endregion
        }

        /// <summary>
        /// <para>The other object</para>
        /// </summary>
        public sealed class Other : GroupType
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<Other> Encoder = new OtherEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<Other> Decoder = new OtherDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="Other" /> class.</para>
            /// </summary>
            private Other()
            {
            }

            /// <summary>
            /// <para>A singleton instance of Other</para>
            /// </summary>
            public static readonly Other Instance = new Other();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="Other" />.</para>
            /// </summary>
            private class OtherEncoder : enc.StructEncoder<Other>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(Other value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="Other" />.</para>
            /// </summary>
            private class OtherDecoder : enc.StructDecoder<Other>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="Other" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override Other Create()
                {
                    return new Other();
                }

                /// <summary>
                /// <para>Decode fields without ensuring start and end object.</para>
                /// </summary>
                /// <param name="reader">The json reader.</param>
                /// <returns>The decoded object.</returns>
                public override Other DecodeFields(enc.IJsonReader reader)
                {
                    return Other.Instance;
                }
            }

            #endregion
        }
    }
}
