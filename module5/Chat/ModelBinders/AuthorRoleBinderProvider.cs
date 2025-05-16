using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Chat.ModelBinders
{
    public class AuthorRoleBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Metadata.ModelType == typeof(AuthorRole))
                return new BinderTypeModelBinder(typeof(AuthorRoleModelBinder));

            return null;
        }
    }
}
