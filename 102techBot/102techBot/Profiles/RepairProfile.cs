using _102techBot.Domain;
using _102techBot.Domain.Entities;
using AutoMapper;

namespace _102techBot.Profiles
{
    internal class RepairProfile : Profile
    {
        public RepairProfile()
        {
            CreateMap<TemporaryRepair, Repair>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => ValidateUserId(src.UserId)))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => ValidateUser(src.User)))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => ValidatePhone(src.Phone)))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => ValidateProductId(src.ProductId)))
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => ValidateProduct(src.Product)))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => ValidateCategoryId(src.CategoryId)))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => ValidateCategory(src.Category)))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => ValidateDescription(src.Description)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
        }

        private long ValidateUserId(long? userId)
        {
            if (!userId.HasValue || userId.Value == 0)
            {
                throw new ArgumentNullException(nameof(userId), "UserId is required.");
            }
            return userId.Value;
        }

        private User ValidateUser(User? user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User is required.");
            }
            return user;
        }

        private string ValidatePhone(string? phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                throw new ArgumentNullException(nameof(phone), "Phone is required.");
            }
            return phone;
        }

        private long ValidateProductId(long? productId)
        {
            if (!productId.HasValue || productId.Value == 0)
            {
                throw new ArgumentNullException(nameof(productId), "ProductId is required.");
            }
            return productId.Value;
        }

        private Product ValidateProduct(Product? product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product), "Product is required.");
            }
            return product;
        }

        private long ValidateCategoryId(long? categoryId)
        {
            if (!categoryId.HasValue || categoryId.Value == 0)
            {
                throw new ArgumentNullException(nameof(categoryId), "CategoryId is required.");
            }
            return categoryId.Value;
        }

        private Category ValidateCategory(Category? category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category), "Category is required.");
            }
            return category;
        }

        private string ValidateDescription(string? description)
        {
            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentNullException(nameof(description), "Description is required.");
            }
            return description;
        }
    }
}
