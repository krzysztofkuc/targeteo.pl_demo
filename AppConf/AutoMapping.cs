using AutoMapper;
using System;
using System.Web;
using targeteo.pl.Common;
using targeteo.pl.Model.Entities;
using targeteo.pl.Model.ViewModel;

namespace targeteo.pl.AppConf
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            AllowNullCollections = true;
            AllowNullDestinationValues = true;

            //Category
            CreateMap<Category, MenuItem>().MaxDepth(3)
            .ForMember(dest => dest.label, opt => opt.MapFrom(src => src.Caption))
            .ForMember(dest => dest.ThumbnailFileName, opt => opt.MapFrom(src => src.ThumbnailFileName))
            .ForMember(dest => dest.items, opt => opt.MapFrom(src => src.InverseParent.Count != 0 ? src.InverseParent : null))
            .ForMember(dest => dest.routerLink, opt => opt.MapFrom(src => src.Parent == null ? new object[] { "","kategoria", src.Name } : new object[] { "", "kategoria", src.Parent.Name , src.Name }));

            CreateMap<Category, TreeNodeVm>().MaxDepth(10)
            .ForMember(dest => dest.label, opt => opt.MapFrom(src => src.Caption))
            .ForMember(dest => dest.children, opt => opt.MapFrom(src => src.InverseParent))
            .ForMember(dest => dest.data, opt => opt.MapFrom(src => src.CategoryId ))
            //.ForMember(dest => dest.icon, opt => opt.MapFrom(src => src.Icon))
            
            .ForMember(dest => dest.routerLink, opt => opt.MapFrom(src => src.Parent == null ? new object[] { "", "kategoria", src.Name } : new object[] { "", "kategoria", src.Parent.Name, src.Name })); ;

            //Category
            CreateMap<CategoryVM, Category>().MaxDepth(3)
             .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Products))
             .ForMember(dest => dest.CategoryAttributes, opt => opt.MapFrom(src => src.Attributes))
            .ForMember(dest => dest.InverseParent, opt => opt.MapFrom(src => src.Categories));

            CreateMap<Category, CategoryVM>().MaxDepth(3)
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Product))
            .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.CategoryAttributes))
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.InverseParent));

            //Product
            CreateMap<ProductVM, Product>().MaxDepth(10);
            CreateMap<Product, ProductVM>().MaxDepth(10);

            CreateMap<ProductMainPageVM, Product>().MaxDepth(10);
            CreateMap<Product, ProductMainPageVM>().MaxDepth(10);

            //Cart
            CreateMap<CartVM, Cart>();
            CreateMap<Cart, CartVM>();

            //Order
            CreateMap<OneUserOrderVM, OneUserOrder>();
            CreateMap<OneUserOrder, OneUserOrderVM>();

            //Order
            CreateMap<CompanyInformationVM, CompanyInformation>();
            CreateMap<CompanyInformation, CompanyInformationVM>();

            //ProductAttributes
            CreateMap<CategoryAttributeVM, AddProductAttributeVM>()
            .ForMember(x => x.AllAttributeTypes, opts => opts.Ignore())
            .ForMember(x => x.AllCategories, opts => opts.Ignore());

            CreateMap<AddProductAttributeVM, CategoryAttributeVM>();

            //CategoryAttributeVM
            CreateMap<CategoryAttributeVM, CategoryAttributes>();
            CreateMap<CategoryAttributes, CategoryAttributeVM>();

            //PictureVM
            CreateMap<PictureVM, Picture>();
            CreateMap<Picture, PictureVM>();

            //AttributeValueList
            CreateMap<AttributeValueListVM, AttributeValueList>();
            CreateMap<AttributeValueList, AttributeValueListVM>();

            //ProductAttribute
            CreateMap<ProductAttributeVM, ProductAttribute>().MaxDepth(3);

            CreateMap<ProductAttribute, ProductAttributeVM>().MaxDepth(3)
                .ForMember(x => x.ComboboxValues, opts => opts.Ignore());

            //ProductAttribute to AddProducTAttribute
            CreateMap<ProductAttributeVM, AddProductAttributeVM>().MaxDepth(3);
            CreateMap<AddProductAttributeVM, ProductAttributeVM>().MaxDepth(3);

            //Create categoryAttribute - AddCategoryAttrribute to CategoryAttribute
            CreateMap<CategoryAttributeVM, AddCategoryAttributeVM>()
            .ForMember(x => x.PKAttributeId, opt => opt.Ignore())
            .ForMember(x => x.ProductAttributes, opt => opt.Ignore());

            CreateMap<AddCategoryAttributeVM, CategoryAttributeVM>()
            .ForMember(x => x.CategoryAttributeId, opt => opt.MapFrom(src => src.Category.CategoryId))
            .ForMember(x => x.Id, opt => opt.Ignore())
            //.ForMember(x => x.ProductAttributes, opt => opt.Ignore())
            .ForMember(x => x.Bit, opt => opt.MapFrom(src => src.AttributeType == nameof(Enums.AllAttributeTypes.bit) ? Convert.ToBoolean(src.Value) : false));

            //Address
            CreateMap<AdressVM, Adress>();
            CreateMap<Adress, AdressVM>();

            //SupplyMethod
            CreateMap<SupplyMethodVm, SupplyMethods>();
            CreateMap<SupplyMethods, SupplyMethodVm>();

            //OrderDeatil
            CreateMap<OrderDetailVM, OrderDetail>();
            CreateMap<OrderDetail, OrderDetailVM>();

            //OrderSummaryVm
            CreateMap<OrderSummaryVm, OrderSummary>();
            CreateMap<OrderSummary, OrderSummaryVm>();
            

            //Complete order
            //CreateMap<OneUserOrderVM, OrderVM>()
            //.ForMember(x => x.FirstName, opt => opt.MapFrom(src => src.FirstName))
            //.ForMember(x => x.LastName, opt => opt.MapFrom(src => src.LastName))
            //.ForMember(x => x.Address, opt => opt.MapFrom(src => src.Address))
            //.ForMember(x => x.City, opt => opt.MapFrom(src => src.City))
            //.ForMember(x => x.State, opt => opt.MapFrom(src => src.State))
            //.ForMember(x => x.FirstName, opt => opt.MapFrom(src => src.FirstName))
            //.ForMember(x => x.PostalCode, opt => opt.MapFrom(src => src.PostalCode))
            //.ForMember(x => x.PostalCode, opt => opt.MapFrom(src => src.PostalCode))
            //.ForMember(x => x.Phone, opt => opt.MapFrom(src => src.Phone))
            //.ForMember(x => x.OrderDetails, opt => opt.MapFrom(src => src.Items))
            //.ForMember(x => x.OrderId, opt => opt.MapFrom(src => src.OrderId));
            //CreateMap<OrderVM, CompleteOrderVM>();+

            CreateMap<MsgToUserVm, Comments>();
            CreateMap<CommentVm, Comments>();
            CreateMap<Comments, CommentVm>();
            CreateMap<Users, UserVM>();
            CreateMap<UserVM, Users>();

            CreateMap<City, CityVm>();
            CreateMap<CityVm, City>();

            CreateMap<SupplyMethodVm, SupplyMethods>();
            //CreateMap<SupplyMethodEntity, SupplyMethodVm>();

            CreateMap<AnnouncementPayment, AnnouncementPaymentVm>();
            CreateMap<AnnouncementPaymentVm, AnnouncementPayment>();

            CreateMap<OrderOpinion, OrderOpinionVM>();
            CreateMap<OrderOpinionVM, OrderOpinion>();

            CreateMap<UserSupplyMethod, UserSupplyMethodVm>();
            CreateMap<UserSupplyMethodVm, UserSupplyMethod>()
                .ForMember(x => x.SupplyMethod, opt => opt.Ignore());

            CreateMap<UserAccount, UserAccountVM>();
            CreateMap<UserAccountVM, UserAccount>();

            CreateMap<AccountOperationStatus, AccountOperationStatusVM>();
            CreateMap<AccountOperationStatusVM, AccountOperationStatus>();



        }
    }
}
