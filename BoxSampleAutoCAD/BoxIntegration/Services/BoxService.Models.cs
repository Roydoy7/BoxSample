using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BoxSampleAutoCAD.BoxIntegration.Services
{
    public class BoxFile
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public abstract class BoxCollection
    {
        public const string FieldTotalCount = "total_count";
        public const string FieldEntries = "entries";
        public const string FieldOffset = "offset";
        public const string FieldLimit = "limit";
        public const string FieldOrder = "order";
        public const string FieldNextMarker = "next_marker";
    }

    public class BoxSortOrder
    {
        [JsonProperty(PropertyName = "by")]
        public virtual BoxSortBy By { get; private set; }

        [JsonProperty(PropertyName = "sort")]
        public virtual string Sort { get; private set; }

        [JsonProperty(PropertyName = "direction")]
        public virtual BoxSortDirection Direction { get; private set; }
    }

    /// <summary>
    /// The way an item collection is ordered by
    /// </summary>
    public enum BoxSortBy
    {
        Type,
        Name,
        file_version_id,
        Id,
        policy_name,
        retention_policy_id,
        retention_policy_object_id,
        retention_policy_set_id,
        interacted_at
    }

    /// <summary>
    /// The sort direction of an item collection
    /// </summary>
    public enum BoxSortDirection
    {
        ASC,
        DESC
    }

    public class BoxCollection<T> : BoxCollection where T : class, new()
    {
        [JsonProperty(PropertyName = FieldTotalCount)]
        public virtual int TotalCount { get; set; }

        [JsonProperty(PropertyName = FieldEntries)]
        public virtual List<T> Entries { get; set; }

        [JsonProperty(PropertyName = FieldOffset)]
        public virtual int Offset { get; set; }

        [JsonProperty(PropertyName = FieldLimit)]
        public virtual int Limit { get; set; }

        [JsonProperty(PropertyName = FieldOrder)]
        public virtual List<BoxSortOrder> Order { get; set; }

    }

    public class BoxItem : BoxEntity
    {
        public const string FieldSequence = "sequence_id";
        public const string FieldEtag = "etag";
        public const string FieldName = "name";
        public const string FieldCreatedAt = "created_at";
        public const string FieldModifiedAt = "modified_at";
        public const string FieldTrashedAt = "trashed_at";
        public const string FieldDescription = "description";
        public const string FieldSize = "size";
        public const string FieldPathCollection = "path_collection";
        public const string FieldCreatedBy = "created_by";
        public const string FieldModifiedBy = "modified_by";
        public const string FieldOwnedBy = "owned_by";
        public const string FieldSharedLink = "shared_link";
        public const string FieldParent = "parent";
        public const string FieldItemStatus = "item_status";
        public const string FieldPermissions = "permissions";
        public const string FieldTags = "tags";

        /// <summary>
        /// A unique ID for use with the /events endpoint
        /// </summary>
        [JsonProperty(PropertyName = FieldSequence)]
        public virtual string SequenceId { get; private set; }

        /// <summary>
        /// A unique string identifying the version of this item
        /// </summary>
        [JsonProperty(PropertyName = FieldEtag)]
        public virtual string ETag { get; private set; }

        /// <summary>
        /// The name of the item
        /// </summary>
        [JsonProperty(PropertyName = FieldName)]
        public virtual string Name { get; private set; }

        /// <summary>
        /// The description of the item
        /// </summary>
        [JsonProperty(PropertyName = FieldDescription)]
        public virtual string Description { get; private set; }

        /// <summary>
        /// The folder size in bytes
        /// </summary>
        [JsonProperty(PropertyName = FieldSize)]
        public virtual long? Size { get; private set; }

        /// <summary>
        /// The path of folders to this item, starting at the root
        /// </summary>
        [JsonProperty(PropertyName = FieldPathCollection)]
        public virtual BoxCollection<BoxFolder> PathCollection { get; private set; }

        /// <summary>
        /// The time the item was created
        /// </summary>
        [JsonProperty(PropertyName = FieldCreatedAt)]
        public virtual DateTimeOffset? CreatedAt { get; private set; }

        /// <summary>
        /// The time the item or its contents were last modified
        /// </summary>
        [JsonProperty(PropertyName = FieldModifiedAt)]
        public virtual DateTimeOffset? ModifiedAt { get; private set; }

        /// <summary>
        /// The time at which this item was put in the trash.
        /// </summary>
        [JsonProperty(PropertyName = FieldTrashedAt)]
        public virtual DateTimeOffset? TrashedAt { get; set; }

        /// <summary>
        /// The user who created this item
        /// </summary>
        [JsonProperty(PropertyName = FieldCreatedBy)]
        public virtual BoxUser CreatedBy { get; private set; }

        /// <summary>
        /// The user who last modified this item
        /// mini user object
        /// </summary>
        [JsonProperty(PropertyName = FieldModifiedBy)]
        public virtual BoxUser ModifiedBy { get; private set; }

        /// <summary>
        /// The user who owns this item
        /// mini user object
        /// </summary>
        [JsonProperty(PropertyName = FieldOwnedBy)]
        public virtual BoxUser OwnedBy { get; private set; }

        /// <summary>
        /// The folder that contains this one
        /// </summary>
        [JsonProperty(PropertyName = FieldParent)]
        public virtual BoxFolder Parent { get; private set; }

        /// <summary>
        /// Whether this item is deleted or not
        /// </summary>
        [JsonProperty(PropertyName = FieldItemStatus)]
        public virtual string ItemStatus { get; private set; }

        /// <summary>
        /// The tag for this item
        /// </summary>
        [JsonProperty(PropertyName = FieldTags)]
        public virtual string[] Tags { get; private set; }
    }

    public class BoxFolderPermission : BoxItemPermission
    {

        /// <summary>
        /// Permission to invite additional users to be collaborators
        /// </summary>
        [JsonProperty(PropertyName = "can_invite_collaborator")]
        public bool CanInviteCollaborator { get; private set; }

    }

    public class BoxItemPermission
    {
        /// <summary>
        /// Permission to download item
        /// </summary>
        [JsonProperty(PropertyName = "can_download")]
        public bool CanDownload { get; private set; }

        /// <summary>
        /// Permission to upload item
        /// </summary>
        [JsonProperty(PropertyName = "can_upload")]
        public bool CanUpload { get; private set; }

        /// <summary>
        /// Permission to comment on item
        /// </summary>
        [JsonProperty(PropertyName = "can_comment")]
        public bool CanComment { get; private set; }

        /// <summary>
        /// Permission to rename the item
        /// </summary>
        [JsonProperty(PropertyName = "can_rename")]
        public bool CanRename { get; private set; }

        /// <summary>
        /// Permission to delete the item
        /// </summary>
        [JsonProperty(PropertyName = "can_delete")]
        public bool CanDelete { get; private set; }

        /// <summary>
        /// Permission to share item
        /// </summary>
        [JsonProperty(PropertyName = "can_share")]
        public bool CanShare { get; private set; }

        /// <summary>
        /// Permission to change the access on the share
        /// </summary>
        [JsonProperty(PropertyName = "can_set_share_access")]
        public bool CanSetShareAccess { get; private set; }
    }

    /// <summary>
    /// Box representation of a folder
    /// </summary>
    public class BoxFolder : BoxItem
    {
        public const string FieldFolderUploadEmail = "folder_upload_email";
        public const string FieldItemCollection = "item_collection";
        public const string FieldSyncState = "sync_state";
        public const string FieldHasCollaborations = "has_collaborations";
        public const string FieldAllowedInviteeRoles = "allowed_invitee_roles";
        public const string FieldWatermarkInfo = "watermark_info";
        public const string FieldPurgedAt = "purged_at";
        public const string FieldContentCreatedAt = "content_created_at";
        public const string FieldContentModifiedAt = "content_modified_at";
        public const string FieldCanNonOwnersInvite = "can_non_owners_invite";
        public const string FieldIsExternallyOwned = "is_externally_owned";
        public const string FieldAllowedSharedLinkAccessLevels = "allowed_shared_link_access_levels";
        public const string FieldExpiresAt = "expires_at";
        public const string FieldIsCollaborationRestrictedToEnterprise = "is_collaboration_restricted_to_enterprise";
        public const string FieldClassification = "classification";

        /// <summary>
        /// The upload email address for this folder
        /// </summary>
        [JsonProperty(PropertyName = FieldFolderUploadEmail)]
        public virtual BoxEmail FolderUploadEmail { get; private set; }

        /// <summary>
        /// A collection of mini file and folder objects contained in this folder
        /// </summary>
        [JsonProperty(PropertyName = FieldItemCollection)]
        public virtual BoxCollection<BoxItem> ItemCollection { get; private set; }

        /// <summary>
        /// Indicates whether this folder will be synced by the Box sync clients or not. Can be synced, not_synced, or partially_synced
        /// </summary>
        [JsonProperty(PropertyName = FieldSyncState)]
        public virtual string SyncState { get; private set; }

        /// <summary>
        /// Indicates whether this folder is a collaboration folder or not
        /// </summary>
        [JsonProperty(PropertyName = FieldHasCollaborations)]
        public virtual bool? HasCollaborations { get; private set; }

        /// <summary>
        /// The available permissions on this folder
        /// </summary>
        [JsonProperty(PropertyName = FieldPermissions)]
        public virtual BoxFolderPermission Permissions { get; protected set; }

        /// <summary>
        /// The available roles that can be used to invite people to the folder
        /// WARNING: This property is still in development and may change!
        /// </summary>
        [JsonProperty(PropertyName = FieldAllowedInviteeRoles)]
        public virtual IList<string> AllowedInviteeRoles { get; protected set; }

        /// <summary>
        /// Metadata on this file.
        /// </summary>
        [JsonProperty(PropertyName = "metadata")]
        public virtual dynamic Metadata { get; protected set; }

        /// <summary>
        /// Purged at timestamp for folder
        /// </summary>
        [JsonProperty(PropertyName = FieldPurgedAt)]
        public virtual DateTimeOffset? PurgedAt { get; set; }

        /// <summary>
        /// Content created at timestamp for folder
        /// </summary>
        [JsonProperty(PropertyName = FieldContentCreatedAt)]
        public virtual DateTimeOffset? ContentCreatedAt { get; set; }

        /// <summary>
        /// Content modified at timestamp for folder
        /// </summary>
        [JsonProperty(PropertyName = FieldContentModifiedAt)]
        public virtual DateTimeOffset? ContentModifiedAt { get; set; }

        /// <summary>
        /// Can owners invite field for folder
        /// </summary>
        [JsonProperty(PropertyName = FieldCanNonOwnersInvite)]
        public virtual bool? CanNonOwnersInvite { get; set; }

        /// <summary>
        /// Allowed shared link access levels for folder
        /// </summary>
        [JsonProperty(PropertyName = FieldAllowedSharedLinkAccessLevels)]
        public virtual IList<string> AllowedSharedLinkAccessLevels { get; set; }

        /// <summary>
        /// Is folder externally owned
        /// </summary>
        [JsonProperty(PropertyName = FieldIsExternallyOwned)]
        public virtual bool? IsExternallyOwned { get; set; }

        /// <summary>
        /// The date when the folder will be automatically deleted due to item expiration settings.
        /// </summary>
        [JsonProperty(PropertyName = FieldExpiresAt)]
        public virtual DateTimeOffset? ExpiresAt { get; protected set; }

        /// <summary>
        /// The date when the folder will be automatically deleted due to item expiration settings.
        /// </summary>
        [JsonProperty(PropertyName = FieldIsCollaborationRestrictedToEnterprise)]
        public virtual bool? IsCollaborationRestrictedToEnterprise { get; protected set; }

    }

    /// <summary>
    /// Box representation of an email
    /// </summary>
    public class BoxEmail
    {
        public const string FieldAccess = "access";
        public const string FieldEmail = "email";

        /// <summary>
        /// The available access
        /// </summary>
        [JsonProperty(PropertyName = FieldAccess)]
        public virtual string Acesss { get; private set; }

        /// <summary>
        /// The email address
        /// </summary>
        [JsonProperty(PropertyName = FieldEmail)]
        public virtual string Address { get; private set; }
    }

    /// <summary>
    /// Box representation of a user
    /// </summary>
    public class BoxUser : BoxEntity
    {
        public const string FieldName = "name";
        public const string FieldLogin = "login";
        public const string FieldCreatedAt = "created_at";
        public const string FieldModifiedAt = "modified_at";
        public const string FieldRole = "role";
        public const string FieldLanguage = "language";
        public const string FieldSpaceAmount = "space_amount";
        public const string FieldSpaceUsed = "space_used";
        public const string FieldMaxUploadSize = "max_upload_size";
        public const string FieldTrackingCodes = "tracking_codes";
        public const string FieldCanSeeManagedUsers = "can_see_managed_users";
        public const string FieldIsSyncEnabled = "is_sync_enabled";
        public const string FieldStatus = "status";
        public const string FieldJobTitle = "job_title";
        public const string FieldPhone = "phone";
        public const string FieldAddress = "address";
        public const string FieldAvatarUrl = "avatar_url";
        public const string FieldIsExemptFromDeviceLimits = "is_exempt_from_device_limits";
        public const string FieldIsExemptFromLoginVerification = "is_exempt_from_login_verification";
        public const string FieldEnterprise = "enterprise";
        public const string FieldIsPlatformAccessOnly = "is_platform_access_only";
        public const string FieldTimezone = "timezone";
        public const string FieldIsExternalCollabRestricted = "is_external_collab_restricted";
        public const string FieldMyTags = "my_tags";
        public const string FieldHostname = "hostname";
        public const string FieldExternalAppUserId = "external_app_user_id";
        public const string FieldNotificationEmail = "notification_email";

        /// <summary>
        /// The name of this user
        /// </summary>
        [JsonProperty(PropertyName = FieldName)]
        public virtual string Name { get; private set; }

        /// <summary>
        /// The email address this user uses to login
        /// </summary>
        [JsonProperty(PropertyName = FieldLogin)]
        public virtual string Login { get; private set; }

        /// <summary>
        /// The time this user was created
        /// </summary>
        [JsonProperty(PropertyName = FieldCreatedAt)]
        public virtual DateTimeOffset? CreatedAt { get; private set; }

        /// <summary>
        /// The time this user was last modified
        /// </summary>
        [JsonProperty(PropertyName = FieldModifiedAt)]
        public virtual DateTimeOffset? ModifiedAt { get; private set; }

        /// <summary>
        /// This user’s enterprise role. Can be admin, coadmin, or user
        /// </summary>
        [JsonProperty(PropertyName = FieldRole)]
        public virtual string Role { get; private set; }

        /// <summary>
        /// The language of this user
        /// </summary>
        [JsonProperty(PropertyName = FieldLanguage)]
        public virtual string Language { get; private set; }

        /// <summary>
        /// The user’s total available space amount in bytes
        /// </summary>
        [JsonProperty(PropertyName = FieldSpaceAmount)]
        public virtual long? SpaceAmount { get; private set; }

        /// <summary>
        /// The amount of space in use by the user
        /// </summary>
        [JsonProperty(PropertyName = FieldSpaceUsed)]
        public virtual long? SpaceUsed { get; private set; }

        /// <summary>
        /// The maximum individual file size in bytes this user can have
        /// </summary>
        [JsonProperty(PropertyName = FieldMaxUploadSize)]
        public virtual long? MaxUploadSize { get; private set; }

        /// <summary>
        /// An array of key/value pairs set by the user’s admin
        /// </summary>
        [JsonProperty(PropertyName = FieldTrackingCodes)]
        public virtual IList<BoxTrackingCode> TrackingCodes { get; private set; }

        /// <summary>
        /// Whether this user can see other enterprise users in its contact list
        /// </summary>
        [JsonProperty(PropertyName = FieldCanSeeManagedUsers)]
        public virtual bool? CanSeeManagedUsers { get; private set; }

        /// <summary>
        /// Whether or not this user can use Box Sync
        /// </summary>
        [JsonProperty(PropertyName = FieldIsSyncEnabled)]
        public virtual bool? IsSyncEnabled { get; private set; }

        /// <summary>
        /// Can be active or inactive
        /// </summary>
        [JsonProperty(PropertyName = FieldStatus)]
        public virtual string Status { get; private set; }

        /// <summary>
        /// The user’s job title
        /// </summary>
        [JsonProperty(PropertyName = FieldJobTitle)]
        public virtual string JobTitle { get; private set; }

        /// <summary>
        /// The user’s phone number
        /// </summary>
        [JsonProperty(PropertyName = FieldPhone)]
        public virtual string Phone { get; private set; }

        /// <summary>
        /// The user’s address
        /// </summary>
        [JsonProperty(PropertyName = FieldAddress)]
        public virtual string Address { get; private set; }

        /// <summary>
        /// URL of this user’s avatar image
        /// </summary>
        [JsonProperty(PropertyName = FieldAvatarUrl)]
        public virtual string AvatarUrl { get; private set; }

        /// <summary>
        /// Whether to exempt this user from Enterprise device limits
        /// </summary>
        [JsonProperty(PropertyName = FieldIsExemptFromDeviceLimits)]
        public virtual bool IsExemptFromDeviceLimits { get; private set; }

        /// <summary>
        /// Whether or not this user must use two-factor authentication
        /// </summary>
        [JsonProperty(PropertyName = FieldIsExemptFromLoginVerification)]
        public virtual bool IsExemptFromLoginVerification { get; private set; }

        /// <summary>
        /// Mini representation of this user’s enterprise, including the ID of its enterprise
        /// </summary>
        [JsonProperty(PropertyName = FieldEnterprise)]
        public virtual BoxEnterprise Enterprise { get; private set; }

        /// <summary>
        /// Whether or not the user is an App User (platform)
        /// </summary>
        [JsonProperty(PropertyName = FieldIsPlatformAccessOnly)]
        public virtual bool? IsPlatformAccessOnly { get; private set; }

        /// <summary>
        /// The user's timezone
        /// </summary>
        [JsonProperty(PropertyName = FieldTimezone)]
        public virtual string Timezone { get; private set; }

        /// <summary>
        /// Whether the user has been restricted from collaborating with parties outside their enterprise
        /// </summary>
        [JsonProperty(PropertyName = FieldIsExternalCollabRestricted)]
        public virtual bool? IsExternalCollabRestricted { get; private set; }

        /// <summary>
        /// Tags for all files and folders owned by the user
        /// </summary>
        [JsonProperty(PropertyName = FieldMyTags)]
        public virtual string[] Tags { get; private set; }

        /// <summary>
        /// The root (protocol, subdomain, domain) of any Box URLs that need to be generated for the user
        /// </summary>
        [JsonProperty(PropertyName = FieldHostname)]
        public virtual string Hostname { get; private set; }

        /// <summary>
        /// The external app user id that has been set for the app user.  An arbitrary identifier that can be used by external user sync tools to link this Box User to an external user.
        /// Example values of this field could be an Active Directory Object ID or primary key from a user-tracking database. We recommend use of this field in order to avoid issues when email addresses and names are updated in either Box or external systems.
        /// </summary>
        [JsonProperty(PropertyName = FieldExternalAppUserId)]
        public virtual string ExternalAppUserId { get; private set; }

    }

    /// <summary>
    /// Box mini representation of a enterprise
    /// </summary>
    public class BoxEnterprise : BoxEntity
    {
        public const string FieldName = "name";

        /// <summary>
        /// The name of this enterprise
        /// </summary>
        [JsonProperty(PropertyName = FieldName)]
        public virtual string Name { get; private set; }
    }

    /// <summary>
    /// Box representation of a tracking code
    /// </summary>
    public class BoxTrackingCode
    {
        public const string FieldType = "type";
        public const string FieldName = "name";
        public const string FieldValue = "value";

        /// <summary>
        /// Constructor for creating new BoxTrackingCodes with a given name and value, such that they can be created and passed in BoxUserRequests
        /// </summary>
        /// <param name="name">Name of a tracking code registered by the enterprise administrator.</param>
        /// <param name="value">Value of the tracking code.</param>
        public BoxTrackingCode(string name, string value)
        {
            //Per description below, this should always be tracking_code
            Type = "tracking_code";
            Name = name;
            Value = value;
        }

        /// <summary>
        /// The type of the tracking code, should be tracking_code
        /// </summary>
        [JsonProperty(PropertyName = FieldType)]
        public virtual string Type { get; private set; }

        /// <summary>
        /// The name of the tracking code
        /// </summary>
        [JsonProperty(PropertyName = FieldName)]
        public virtual string Name { get; private set; }

        /// <summary>
        /// The value of the tracking code
        /// </summary>
        [JsonProperty(PropertyName = FieldValue)]
        public virtual string Value { get; private set; }
    }

    /// <summary>
    /// Represents the base class for most Box model objects
    /// </summary>
    public class BoxEntity
    {
        // Marked private as Type and ID are always returned with every response regardless of included Fields
        private const string FieldType = "type";
        private const string FieldId = "id";

        /// <summary>
        /// The item’s ID
        /// </summary>
        [JsonProperty(PropertyName = FieldId)]
        public virtual string Id { get; protected set; }

        /// <summary>
        /// The type of the item
        /// </summary>
        [JsonProperty(PropertyName = FieldType)]
        public virtual string Type { get; protected set; }
    }
}
