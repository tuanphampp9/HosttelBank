using Microsoft.AspNetCore.Mvc;
using Btl_web_nc.Models;
using Btl_web_nc.RepositoryInterfaces;

[MyAuthenFilter]
[AdminAuthorFilter]
public class PostManageController : Controller
{

    private readonly IPostRepositories _postRepository;

    public PostManageController(IPostRepositories postRepository)
    {
        _postRepository = postRepository;
    }

    // Action để hiển thị danh sách bài đăng
    public IActionResult Index()
    {

        var posts = _postRepository.GetAllPosts();
        return View(posts);
    }

    [HttpGet]
    public IActionResult PostDetail(long postId){

        return RedirectToAction("PostDetail", "Post", new { postId });
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

    // Action để xóa bài đăng
    [HttpPost]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            // Gọi phương thức xóa bài đăng từ repository và nhận kết quả trả về
            bool result = await _postRepository.DeletePostAsync(id);

            if (result)
            {
                // Nếu xóa thành công, chuyển hướng đến trang danh sách bài đăng với thông báo thành công
                TempData["SuccessMessage"] = "Bài đăng đã được xóa thành công.";
            }
            else
            {
                // Nếu xóa không thành công, chuyển hướng đến trang danh sách bài đăng với thông báo lỗi
                TempData["ErrorMessage"] = "Không thể xóa bài đăng. Vui lòng thử lại.";
            }

            return RedirectToAction("Index");
        }
        catch (Exception)
        {
            // Xử lý lỗi nếu có
            TempData["ErrorMessage"] = "Xóa bài đăng không thành công. Vui lòng thử lại.";
            return RedirectToAction("Index");
        }
    }
}