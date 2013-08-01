﻿/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>File Group</b> entity.
    /// </summary>
    public partial class FileGroup : Entity
    {
        #region Member Variables

        // --- Background storage for properties ---
        private FileGroupType fileGroupType;
        private FileGroupLayoutType layoutType;
        private FileGroupAllocationType allocationType;
        private DiskVolumeType diskVolumeType;
        private string fileGroupName;
        private long allocatedSpace;
        private int fileCount;

        #endregion
        #region Member Access Properties

        /// <summary>
        /// Gets or sets the value determining whether the file group is a definition for
        /// data file or log files.
        /// </summary>
        [DBColumn]
        public FileGroupType FileGroupType
        {
            get { return fileGroupType; }
            set { fileGroupType = value; }
        }

        /// <summary>
        /// Gets or sets the value determining whether the file group is sliced and partitioned
        /// or not.
        /// </summary>
        [DBColumn]
        public FileGroupLayoutType LayoutType
        {
            get { return layoutType; }
            set { layoutType = value; }
        }

        /// <summary>
        /// Gets or sets the value determining the allocation method of the individual database files
        /// over the disk volumes when instantiating physical databases.
        /// </summary>
        [DBColumn]
        public FileGroupAllocationType AllocationType
        {
            get { return allocationType; }
            set { allocationType = value; }
        }

        /// <summary>
        /// Gets or sets the value of the disk volume type to by used when creating physical files.
        /// </summary>
        [DBColumn]
        public DiskVolumeType DiskVolumeType
        {
            get { return diskVolumeType; }
            set { diskVolumeType = value; }
        }

        /// <summary>
        /// Gets or sets the name of the <b>File Group</b>.
        /// </summary>
        /// <remarks>
        /// This refers to the actual file group name in the <b>Schema Template Database</b>.
        /// </remarks>
        [DBColumn(Size = 50)]
        public string FileGroupName
        {
            get { return fileGroupName; }
            set { fileGroupName = value; }
        }

        /// <summary>
        /// Gets or sets the number of bytes to be allocated when the files are created.
        /// </summary>
        [DBColumn]
        public long AllocatedSpace
        {
            get { return allocatedSpace; }
            set { allocatedSpace = value; }
        }

        /// <summary>
        /// Gets or sets the number of individual database file to be created.
        /// </summary>
        [DBColumn]
        public int FileCount
        {
            get { return fileCount; }
            set { fileCount = value; }
        }

        #endregion
        #region Navigation Properties

        /// <summary>
        /// Gets the <b>Database Definition</b> object to which this <b>File Group</b> belongs.
        /// </summary>
        /// <remarks>
        /// This property does do lazy loading, no calling of a loader function is necessary, but
        /// a valid object context with an open database connection must be set.
        /// </remarks>
        public DatabaseDefinition DatabaseDefinition
        {
            get { return (DatabaseDefinition)ParentReference.Value; }
        }

        #endregion
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// The default constructor is required for XML and binary serialization. Do not use this.
        /// </remarks>
        public FileGroup()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>File Group</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public FileGroup(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public FileGroup(Context context, DatabaseDefinition parent)
            : base(context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>File Group</b> objects.
        /// </summary>
        /// <param name="old">The <b>File Group</b> to copy from.</param>
        public FileGroup(FileGroup old)
            : base(old)
        {
            CopyMembers(old);
        }

        #endregion
        #region Initializer Functions

        /// <summary>
        /// Initializes member variables to their initial values.
        /// </summary>
        /// <remarks>
        /// This function is called by the contructors.
        /// </remarks>
        private void InitializeMembers()
        {
            base.EntityType = EntityType.FileGroup;
            base.EntityGroup = EntityGroup.Federation;

            this.FileGroupType = FileGroupType.Unknown;
            this.layoutType = FileGroupLayoutType.Unknown;
            this.allocationType = FileGroupAllocationType.CrossVolume;
            this.diskVolumeType = DiskVolumeType.Data;
            this.fileGroupName = string.Empty;
            this.allocatedSpace = 0;
            this.fileCount = 0;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>File Group</b> object to create the deep copy from.</param>
        private void CopyMembers(FileGroup old)
        {
            this.fileGroupType = old.fileGroupType;
            this.layoutType = old.layoutType;
            this.allocationType = old.allocationType;
            this.diskVolumeType = old.diskVolumeType;
            this.fileGroupName = old.fileGroupName;
            this.allocatedSpace = old.allocatedSpace;
            this.fileCount = old.fileCount;
        }

        #endregion
    }
}
