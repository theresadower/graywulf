﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Registry
{
    public class RegistrySerializer : ContextObject
    {
        private Entity rootEntity;
        private EntityGroup entityGroupMask;
        private EntityType entityTypeMask;
        private bool recursive;
        private bool excludeUserCreated;
        private HashSet<Guid> systemUsers;

        public Entity RootEntity
        {
            get { return rootEntity; }
            set { rootEntity = value; }
        }

        public EntityGroup EntityGroupMask
        {
            get { return entityGroupMask; }
            set { entityGroupMask = value; }
        }

        public EntityType EntityTypeMask
        {
            get { return entityTypeMask; }
            set { entityTypeMask = value; }
        }

        public bool Recursive
        {
            get { return recursive; }
            set { recursive = value; }
        }

        public bool ExcludeUserCreated
        {
            get { return excludeUserCreated; }
            set { excludeUserCreated = value; }
        }

        public RegistrySerializer(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        public RegistrySerializer(Entity rootEntity)
            : base(rootEntity.Context)
        {
            InitializeMembers();

            this.rootEntity = rootEntity;
        }

        private void InitializeMembers()
        {
            this.rootEntity = null;
            this.entityGroupMask = EntityGroup.All;
            this.entityTypeMask = EntityType.All;
            this.recursive = true;
            this.excludeUserCreated = true;
            this.systemUsers = null;
        }

        private void LoadSystemUsers()
        {
            Context.Cluster.LoadDomains(true);
            var sysdom = Context.Cluster.Domains[Constants.SystemDomainName];
            sysdom.LoadUsers(true);
            systemUsers = new HashSet<Guid>();
            systemUsers.Add(Guid.Empty);
            systemUsers.UnionWith(sysdom.Users.Values.Select(u => u.Guid));
        }

        /// <summary>
        /// Serializes an entity and all its child elements into XML.
        /// </summary>
        /// <param name="entity">The root entity of the serialization.</param>
        /// <param name="output">The TextWriter object used for writing the XML stream.</param>
        public void Serialize(TextWriter output)
        {
            if (excludeUserCreated)
            {
                LoadSystemUsers();
            }

            var registry = new Registry();
            registry.Entities = EnumerateChildrenForSerialize(rootEntity).ToArray();

            var ser = new XmlSerializer(registry.GetType());
            ser.Serialize(output, registry);
        }

        /// <summary>
        /// Recursively enumerate entities in the registry tree
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="excludeEntities"></param>
        /// <param name="excludeUserJobs"></param>
        /// <returns></returns>
        /// <remarks>
        /// The function enumerates nodes of a subtree of the registry,
        /// returning parent items first, then immediate children in their
        /// appropriate order by the field 'Number'.
        /// Certain entities are excluded but their children are still
        /// included in the search!
        /// </remarks>
        private IEnumerable<Entity> EnumerateChildrenForSerialize(Entity entity)
        {
            // See if this particular type of entity is included by the masks
            if ((entity.EntityGroup & entityGroupMask) != 0 &&
                (entity.EntityType & entityTypeMask) != 0 &&
                (!excludeUserCreated || systemUsers.Contains(entity.UserGuidOwner)))
            {
                yield return entity;
            }

            if (recursive)
            {
                // Even if it's excluded, return children.
                // Some exports (layout) might require exporting certain security
                // objects (user-mydb mapping)
                entity.LoadAllChildren(true);
                foreach (Entity e in entity.EnumerateAllChildren().OrderBy(ei => ei.Number))
                {
                    foreach (Entity ee in EnumerateChildrenForSerialize(e))
                    {
                        yield return ee;
                    }
                }
            }
        }
    }
}
