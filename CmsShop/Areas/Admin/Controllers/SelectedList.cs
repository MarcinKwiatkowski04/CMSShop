using CmsShop.Models.Data;
using System.Collections.Generic;

namespace CmsShop.Areas.Admin.Controllers
{
    internal class SelectedList
    {
        private List<CategoryDTO> list;

        public SelectedList(List<CategoryDTO> list, string v, string v1)
        {
            this.list = list;
        }
    }
}