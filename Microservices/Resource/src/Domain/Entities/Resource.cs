﻿using System;
using Resource.Domain.Common;

namespace Resource.Domain.Entities
{
    public class Resource : AuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}