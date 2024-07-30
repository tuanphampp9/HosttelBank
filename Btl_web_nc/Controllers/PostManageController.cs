using Microsoft.AspNetCore.Mvc;
using Btl_web_nc.Models;
using Btl_web_nc.RepositoryInterfaces;

[MyAuthenFilter]
[AdminOrPropertyOwnerFilter]
public class PostManageController : Controller
{

    private readonly IPostRepositories _postRepository;
    private readonly INotifyRepositories _notifyRepository;

    public PostManageController(IPostRepositories postRepository, INotifyRepositories notifyRepository)
    {
        _postRepository = postRepository;
        _notifyRepository = notifyRepository;
    }

    // Action để hiển thị danh sách bài đăng
    public IActionResult Index()
    {

        // Lấy userId từ claims của người dùng hiện tại
        var userIdClaim = User.FindFirst("UserId")?.Value;
        var roleName = User.FindFirst("RoleName")?.Value;

        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            // Xử lý khi không tìm thấy userId hoặc userId không hợp lệ
            return Unauthorized("User ID is not valid or not found.");
        }

        if (string.Equals(roleName, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            // Lấy tất cả các bài đăng nếu người dùng là Admin
            var posts = _postRepository.GetAllPosts();
            return View(posts);
        }
        else
        {
            // Lấy các bài đăng của người dùng theo userId nếu người dùng không phải Admin
            var posts = _postRepository.GetPostsByUserId(userId);
            return View(posts);
        }



    }

    // Action để chỉnh sửa bài đăng
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var post = await _postRepository.GetPostViewModelByIdAsync(id);
        if (post == null)
        {
            TempData["ErrorMessage"] = $"Không tìm thấy bài đăng với id {id}. Vui lòng kiểm tra lại.";
            return NotFound();
        }
        return View(post);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, PostManageViewModel post)
    {
        if (id != post.PostId)
        {
            return NotFound(); // Trả về 404 nếu id không khớp với post.Id
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _postRepository.UpdatePostAsync(post);
            }
            catch (Exception)
            {
                throw; // Xử lý ngoại lệ khi cập nhật bị lỗi
            }
            return RedirectToAction(nameof(Index));
        }

        return View(post); // Trả về view 'Edit.cshtml' nếu model không hợp lệ
    }

    
    [HttpPost]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            
            bool result = await _postRepository.DeletePostAsync(id);

            if (result)
            {
              
                TempData["SuccessMessage"] = "Bài đăng đã được xóa thành công.";
            }
            else
            {
                
                TempData["ErrorMessage"] = "Không thể xóa bài đăng. Vui lòng thử lại.";
            }

            return RedirectToAction("Index");
        }
        catch (Exception)
        {
            
            TempData["ErrorMessage"] = "Xóa bài đăng không thành công. Vui lòng thử lại.";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public async Task<IActionResult> ChangeStatus(int postId, string status, string content)
    {
        if (!User.IsInRole("Admin"))
        {
            return Unauthorized();
        }

        var post = await _postRepository.GetPostByIdAsync(postId);

        if (post == null)
        {
            return NotFound(); 
        }

        post.status = status;

        var notify = new Notify
        {
            userId = post.userId, 
            postId = postId,
            content = content
        };
        var postManageViewModel = new PostManageViewModel(post);

        try

        {
            await _postRepository.UpdatePostAsync(postManageViewModel);
            await _notifyRepository.AddNotifyAsync(notify); 

            TempData["SuccessMessage"] = "Trạng thái bài đăng đã được cập nhật và thông báo đã được gửi.";
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật trạng thái. Vui lòng thử lại.";
            
        }

        return RedirectToAction(nameof(Index)); 
    }
}