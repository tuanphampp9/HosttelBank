﻿using Btl_web_nc.Models;

namespace Btl_web_nc.RepositoryInterfaces
{
    public interface IPostRepositories
    {
        public List<Post> GetAllPosts();
        public Task<Post> GetPostByIdAsync(int postId); 
        //public Task<Post> UpdatePostAsync(Post post);
        public Task<PostManageViewModel> GetPostViewModelByIdAsync(int postId);
        public Task<Post> UpdatePostAsync(PostManageViewModel post);
        public Task<Boolean> DeletePostAsync(int postId);
    }
}
