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
    /// <para>The groups get info error object</para>
    /// </summary>
    public class GroupsGetInfoError
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<GroupsGetInfoError> Encoder = new GroupsGetInfoErrorEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<GroupsGetInfoError> Decoder = new GroupsGetInfoErrorDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="GroupsGetInfoError" />
        /// class.</para>
        /// </summary>
        public GroupsGetInfoError()
        {
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is GroupNotOnTeam</para>
        /// </summary>
        public bool IsGroupNotOnTeam
        {
            get
            {
                return this is GroupNotOnTeam;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a GroupNotOnTeam, or <c>null</c>.</para>
        /// </summary>
        public GroupNotOnTeam AsGroupNotOnTeam
        {
            get
            {
                return this as GroupNotOnTeam;
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
        /// <para>Encoder for  <see cref="GroupsGetInfoError" />.</para>
        /// </summary>
        private class GroupsGetInfoErrorEncoder : enc.StructEncoder<GroupsGetInfoError>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(GroupsGetInfoError value, enc.IJsonWriter writer)
            {
                if (value is GroupNotOnTeam)
                {
                    WriteProperty(".tag", "group_not_on_team", writer, enc.StringEncoder.Instance);
                    GroupNotOnTeam.Encoder.EncodeFields((GroupNotOnTeam)value, writer);
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
        /// <para>Decoder for  <see cref="GroupsGetInfoError" />.</para>
        /// </summary>
        private class GroupsGetInfoErrorDecoder : enc.UnionDecoder<GroupsGetInfoError>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="GroupsGetInfoError" />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override GroupsGetInfoError Create()
            {
                return new GroupsGetInfoError();
            }

            /// <summary>
            /// <para>Decode based on given tag.</para>
            /// </summary>
            /// <param name="tag">The tag.</param>
            /// <param name="reader">The json reader.</param>
            /// <returns>The decoded object.</returns>
            protected override GroupsGetInfoError Decode(string tag, enc.IJsonReader reader)
            {
                switch (tag)
                {
                    case "group_not_on_team":
                        return GroupNotOnTeam.Decoder.DecodeFields(reader);
                    default:
                        return Other.Decoder.DecodeFields(reader);
                }
            }
        }

        #endregion

        /// <summary>
        /// <para>The group is not on your team.</para>
        /// </summary>
        public sealed class GroupNotOnTeam : GroupsGetInfoError
        {
            #pragma warning disable 108

            /// <summary>
            /// <para>The encoder instance.</para>
            /// </summary>
            internal static enc.StructEncoder<GroupNotOnTeam> Encoder = new GroupNotOnTeamEncoder();

            /// <summary>
            /// <para>The decoder instance.</para>
            /// </summary>
            internal static enc.StructDecoder<GroupNotOnTeam> Decoder = new GroupNotOnTeamDecoder();

            /// <summary>
            /// <para>Initializes a new instance of the <see cref="GroupNotOnTeam" />
            /// class.</para>
            /// </summary>
            private GroupNotOnTeam()
            {
            }

            /// <summary>
            /// <para>A singleton instance of GroupNotOnTeam</para>
            /// </summary>
            public static readonly GroupNotOnTeam Instance = new GroupNotOnTeam();

            #region Encoder class

            /// <summary>
            /// <para>Encoder for  <see cref="GroupNotOnTeam" />.</para>
            /// </summary>
            private class GroupNotOnTeamEncoder : enc.StructEncoder<GroupNotOnTeam>
            {
                /// <summary>
                /// <para>Encode fields of given value.</para>
                /// </summary>
                /// <param name="value">The value.</param>
                /// <param name="writer">The writer.</param>
                public override void EncodeFields(GroupNotOnTeam value, enc.IJsonWriter writer)
                {
                }
            }

            #endregion

            #region Decoder class

            /// <summary>
            /// <para>Decoder for  <see cref="GroupNotOnTeam" />.</para>
            /// </summary>
            private class GroupNotOnTeamDecoder : enc.StructDecoder<GroupNotOnTeam>
            {
                /// <summary>
                /// <para>Create a new instance of type <see cref="GroupNotOnTeam" />.</para>
                /// </summary>
                /// <returns>The struct instance.</returns>
                protected override GroupNotOnTeam Create()
                {
                    return new GroupNotOnTeam();
                }

                /// <summary>
                /// <para>Decode fields without ensuring start and end object.</para>
                /// </summary>
                /// <param name="reader">The json reader.</param>
                /// <returns>The decoded object.</returns>
                public override GroupNotOnTeam DecodeFields(enc.IJsonReader reader)
                {
                    return GroupNotOnTeam.Instance;
                }
            }

            #endregion
        }

        /// <summary>
        /// <para>The other object</para>
        /// </summary>
        public sealed class Other : GroupsGetInfoError
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
