﻿using Zust.Business.Abstract;
using Zust.DataAccess.Abstract;
using Zust.Entities.Models;

namespace Zust.Business.Concrete
{
    /// <summary>
    /// Represents a service that handles posts.
    /// </summary>
    public class PostService : IPostService
    {
        /// <summary>
        /// Private field representing the data access layer for managing posts.
        /// </summary>
        private readonly IPostDal _postDal;

        /// <summary>
        /// Private field representing the service responsible for user-related operations.
        /// </summary>
        private readonly IUserService _userService;

        /// <summary>
        /// Private field representing the service responsible for handling likes.
        /// </summary>
        private readonly ILikeService _likeService;

        /// <summary>
        /// Private field representing the service responsible for handling comments.
        /// </summary>
        private readonly ICommentService _commentService;

        /// <summary>
        /// Initializes a new instance of the PostService class with the specified dependencies.
        /// </summary>
        /// <param name="postDal">The data access layer for handling posts.</param>
        /// <param name="userService">The service responsible for user-related operations.</param>
        /// <param name="likeService">The service responsible for handling likes.</param>
        /// <param name="commentService">The service responsible for handling comments.</param>
        public PostService(IPostDal postDal, IUserService userService, ILikeService likeService, ICommentService commentService)
        {
            _postDal = postDal;

            _userService = userService;

            _likeService = likeService;

            _commentService = commentService;
        }

        /// <summary>
        /// Adds a new post asynchronously.
        /// </summary>
        /// <param name="post">The Post object representing the post to be added.</param>
        /// <returns>Task representing the asynchronous operation.</returns>
        public async Task AddPostAsync(Post post)
        {
            await _postDal.AddAsync(post);
        }

        /// <summary>
        /// Retrieves all posts for the news feed of a user asynchronously.
        /// </summary>
        /// <param name="currentUserId">The ID of the user whose news feed posts will be retrieved.</param>
        /// <returns>A collection of Post objects representing the posts for the news feed.</returns>
        public async Task<IEnumerable<Post>> GetAllPostsForNewsFeedAsync(string currentUserId)
        {
            var posts = (await GetAllPostsAsync()).Where(p => p.UserId != currentUserId);

            return posts;
        }

        /// <summary>
        /// Retrieves all posts asynchronously.
        /// </summary>
        /// <returns>A collection of Post objects representing all posts.</returns>
        public async Task<IEnumerable<Post>> GetAllPostsAsync()
        {
            var posts = (await _postDal.GetAllAsync()).ToList();

            // Load the user information for each post asynchronously
            await Task.WhenAll(posts.Select(async p => p.User = await _userService.GetUserByIdAsync(p.UserId)));

            return posts;
        }

        /// <summary>
        /// Retrieves all posts of a user asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user whose posts will be retrieved.</param>
        /// <returns>A collection of Post objects representing all posts of the user.</returns>
        public async Task<IEnumerable<Post>> GetAllPostsOfUserAsync(string userId)
        {
            var allPosts = (await GetAllPostsAsync()).Where(p => p.UserId == userId);

            return allPosts;
        }

        /// <summary>
        /// Retrieves a post by its ID asynchronously.
        /// </summary>
        /// <param name="postId">The ID of the post to retrieve.</param>
        /// <returns>The Post object representing the post with the specified ID.</returns>
        public Task<Post?> GetPostByIdAsync(string postId)
        {
            return _postDal.GetAsync(p => p.Id == postId);
        }

        /// <summary>
        /// Gets the total count of likes for all posts of a user asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user to get the total likes count for.</param>
        /// <returns>The total count of likes for all posts of the user.</returns>
        public async Task<int> GetAllPostsLikeCountAsync(string userId)
        {
            var posts = await GetAllPostsOfUserAsync(userId);

            var tasks = posts.Select(p => _likeService.GetPostLikeCountAsync(p.Id));

            int[] likeCounts = await Task.WhenAll(tasks);

            int totalLikesCount = likeCounts.Sum();

            return totalLikesCount;
        }

        /// <summary>
        /// Retrieves the owner of a post by its ID asynchronously.
        /// </summary>
        /// <param name="postId">The ID of the post to retrieve the owner for.</param>
        /// <returns>The User object representing the owner of the post.</returns>
        public async Task<User?> GetOwnerOfPostByIdAsync(string postId)
        {
            var post = await _postDal.GetAsync(p => p.Id == postId);

            var user = await _userService.GetUserByIdAsync(post.UserId);

            return user;
        }

        /// <summary>
        /// Deletes all posts of a user asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user whose posts will be deleted.</param>
        /// <returns>Task representing the asynchronous operation.</returns>
        public async Task DeleteUserPostsAsync(string userId)
        {
            var posts = await _postDal.GetAllAsync(p => p.UserId == userId);

            if (posts != null)
            {
                foreach (var post in posts)
                {
                    await _postDal.DeleteAsync(post);
                }
            }
        }

        /// <summary>
        /// Deletes all comments of a user's posts asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user whose post comments will be deleted.</param>
        /// <returns>Task representing the asynchronous operation.</returns>
        public async Task DeleteUserPostCommentsAsync(string userId)
        {
            var posts = await _postDal.GetAllAsync(p => p.UserId == userId);

            if (posts != null)
            {
                foreach (var post in posts)
                {
                    var comments = await _commentService.GetCommentsOfPostAsync(post.Id);

                    foreach (var comment in comments)
                    {
                        await _commentService.DeleteCommentAsync(comment);
                    }
                }
            }
        }

        /// <summary>
        /// Deletes all likes of a user's posts asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user whose post likes will be deleted.</param>
        /// <returns>Task representing the asynchronous operation.</returns>
        public async Task DeleteUserPostLikesAsync(string userId)
        {
            var posts = await _postDal.GetAllAsync(p => p.UserId == userId);

            if (posts != null)
            {
                foreach (var post in posts)
                {
                    var likes = await _likeService.GetPostLikesAsync(post.Id);

                    foreach (var like in likes)
                    {
                        await _likeService.DeleteLikeAsync(like);
                    }
                }
            }
        }
    }
}