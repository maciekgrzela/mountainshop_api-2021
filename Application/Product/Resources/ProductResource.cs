﻿using System;
using System.Collections.Generic;

namespace Application.Product.Resources
{
    public class ProductResource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int AmountInStorage { get; set; }
        public double NetPrice { get; set; }
        public double PercentageTax { get; set; }
        public double GrossPrice { get; set; }
        public int MinimalOrderedAmount { get; set; }
        public ProducerForProductResource Producer { get; set; }
        public CategoryForProductResource Category { get; set; }
    }
}