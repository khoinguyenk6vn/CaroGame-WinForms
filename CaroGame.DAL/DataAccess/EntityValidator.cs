using System.ComponentModel.DataAnnotations;

namespace CaroGame.DAL.DataAccess
{
    // Kiểm tra (Validate) 1 entity trước khi gọi SaveChangesAsync để ghi xuống CSDL.
    // Dựa vào các thuộc tính [Required], [MaxLength], [Range], [RegularExpression]... khai báo trong Entities.
    // Dữ liệu sai thì ném ValidationException kèm câu báo lỗi (ErrorMessage), không ghi gì xuống CSDL.
    public static class EntityValidator
    {
        public static void Validate(object entity)
        {
            var context = new ValidationContext(entity);
            var errors = new List<ValidationResult>();

            // true: kiểm tra tất cả thuộc tính, không chỉ [Required]
            bool isValid = Validator.TryValidateObject(entity, context, errors, true);
            if (!isValid)
            {
                // Báo lỗi đầu tiên tìm thấy
                throw new ValidationException(errors[0].ErrorMessage);
            }
        }
    }
}
