using Ray.Quartz.Hostloc.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ray.Quartz.Hostloc.Domain;

namespace Ray.Quartz.Hostloc.DomainService
{
    public class PostDomainService : IDomainService
    {
        private readonly IHostlocApi _hostlocApi;

        public PostDomainService(IHostlocApi hostlocApi)
        {
            _hostlocApi = hostlocApi;
        }

        public async Task<Post> GetById(long tid)
        {
            var page = await _hostlocApi.GetPostPageAsync(tid);

            var post=new Post(tid,page.Content);
            return post;
        }
    }
}
