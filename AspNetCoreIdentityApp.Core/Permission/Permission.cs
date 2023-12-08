namespace AspNetCoreIdentity.Core.PermissionRoot
{
    public static class Permission
    {
        public static class Stock
        {
            public const string Read = "Permission.Stock.Read";
            public const string Create = "Permission.Stock.Create";
            public const string Update = "Permission.Stock.Update";
            public const string Delete = "Permission.Stock.Delete";
        }

        public static class Order
        {
            public const string Read = "Permission.Order.Read";
            public const string Create = "Permission.Order.Create";
            public const string Update = "Permission.Order.Update";
            public const string Delete = "Permission.Order.Delete";
        }

        public static class Catalog
        {
            public const string Read = "Permission.Catalog.Read";
            public const string Create = "Permission.Catalog.Create";
            public const string Update = "Permission.Catalog.Update";
            public const string Delete = "Permission.Catalog.Delete";
        }
    }
}