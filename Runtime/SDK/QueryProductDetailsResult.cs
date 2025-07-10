using System;
using System.Collections.Generic;

[System.Serializable]
public class QueryProductDetailsResult
{
    public List<ProductDetails> ProductDetailsList;
    public List<UnfetchedProduct> UnfetchedProductList;
}
