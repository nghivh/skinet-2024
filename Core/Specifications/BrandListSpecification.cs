using System;
using System.Linq.Expressions;
using Core.Entities;
using Core.Interfaces;

namespace Core.Specifications;

public class BrandListSpecification : BaseSpecification<Product, string>
{
    public BrandListSpecification()
    {
        AddSelect(x => x.Brand);
        ApplyDistinct();
    }
}
