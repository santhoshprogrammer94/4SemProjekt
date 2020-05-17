﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Resource.Domain.Entities
{
    public class Resource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<DayAndTime> Available { get; set; }
    }
}