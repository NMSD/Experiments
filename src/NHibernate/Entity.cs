﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernateExperiments.DTOs
{
    public class Entity
    {
        IReadOnlyCollection<EntityItem> readonlyItems;
        public Entity()
        {
            InternalItems = new HashSet<EntityItem>();
            readonlyItems = null;
        }

        public virtual Guid Id { get; set; }

        public virtual string Name { get; set; }

        protected virtual ISet<EntityItem> InternalItems { get; set; }

        //public virtual IReadOnlyCollection<EntityItem> Items
        //{
        //    get
        //    {
        //        if (readonlyItems == null || readonlyItems.Count != InternalItems.Count)
        //        {
        //            readonlyItems = new ReadOnlyCollection<EntityItem>(InternalItems.ToList());
        //        }
        //        return readonlyItems;
        //    }

        //}

        public virtual bool AddItem(EntityItem item)
        {
            item.Parent = this;
            return InternalItems.Add(item);
        }

        public virtual bool RemoveItem(EntityItem item)
        {
            item.Parent = null;
            return InternalItems.Remove(item);
        }
    }

    public class EntityItem : IEquatable<EntityItem>
    {
        public virtual Guid Id { get; set; }

        public virtual string Description { get; set; }

        public virtual Entity Parent { get; set; }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            EntityItem p = obj as EntityItem;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Id == p.Id);
        }

        public virtual bool Equals(EntityItem p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Id == p.Id);
        }

        public override int GetHashCode()
        {
            return 23 ^ Id.GetHashCode();
        }
    }
}