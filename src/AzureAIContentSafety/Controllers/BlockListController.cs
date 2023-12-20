using Azure.AI.ContentSafety;
using AzureAIContentSafety.Helpers;
using AzureAIContentSafety.Interface;
using AzureAIContentSafety.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Reflection;

namespace AzureAIContentSafety.Controllers
{
    [Route("AzureContentSafetyBlockList")]
    public sealed class BlockListController : Controller
    {
        private readonly IAzureAIContentSafetyService _azureAIContentSafetyService;
        private readonly OptimizelyCmsHelpers _optimizelyCmsHelper;

        public BlockListController(
            IAzureAIContentSafetyService azureAIContentSafetyService,
            OptimizelyCmsHelpers optimizelyCmsHelpers)
        {
            _azureAIContentSafetyService = azureAIContentSafetyService;
            _optimizelyCmsHelper = optimizelyCmsHelpers;
        }

        public IActionResult Index()
        {
            var model = new BlockListViewModel();
            model.GetTextBlocklistsPages = _optimizelyCmsHelper.GetTextBlockLists();
            model.BlockLists = _optimizelyCmsHelper.GetTextBlockListsCMS();
            model.GetBlockItems = _optimizelyCmsHelper.GetBlockListItems();
            return View("/Views/Blocklist/Index.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/AzureContentSafetyBlockList/Delete")]
        public ActionResult DeleteBlockList(BlockListViewModel viewModel)
        {
            var model = new BlockListViewModel();
            model.ContentSafetyModel = new ContentSafetyModel();
            model.GetTextBlocklistsPages = _optimizelyCmsHelper.GetTextBlockLists();
            model.GetBlockItems = _optimizelyCmsHelper.GetBlockListItems();
            model.ContentSafetyModel.Message = _azureAIContentSafetyService.DeleteBlockList(viewModel.BlockListName);
            model.BlockLists = _optimizelyCmsHelper.GetTextBlockListsCMS();
            return View("/Views/Index.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/AzureContentSafetyBlockList/CreateBlockList")]
        public ActionResult CreateBlockList(BlockListViewModel viewModel)
        {
            var model = new BlockListViewModel();
            model.ContentSafetyModel = new ContentSafetyModel();
            model.GetTextBlocklistsPages = _optimizelyCmsHelper.GetTextBlockLists();
            model.GetBlockItems = _optimizelyCmsHelper.GetBlockListItems();
            model.BlockLists = _optimizelyCmsHelper.GetTextBlockListsCMS();
            model.ContentSafetyModel.Message = _azureAIContentSafetyService.CreateNewBlockList(viewModel.BlockListName, viewModel.BlockListDescription);
            return View("~/Views/BlockList/Index.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/AzureContentSafetyBlockList/AddBlockItem")]
        public ActionResult AddBlockItem(BlockListViewModel viewModel)
        {
            var model = new BlockListViewModel();
            var listInfos = new List<TextBlockItemInfo>();
            model.ContentSafetyModel = new ContentSafetyModel();
            model.GetBlockItems = _optimizelyCmsHelper.GetBlockListItems();
            model.BlockLists = _optimizelyCmsHelper.GetTextBlockListsCMS();
            model.GetTextBlocklistsPages = _optimizelyCmsHelper.GetTextBlockLists();
            if (viewModel.TextBlockItemInfo.Length > 128)
            {
                model.ContentSafetyModel.Message = "Block Item not added : Please ensure the Block item is 128 characters or less";
            }
            else
            {
                var listInfo = new TextBlockItemInfo(viewModel.TextBlockItemInfo);
                listInfos.Add(listInfo);
                model.ContentSafetyModel.Message = _azureAIContentSafetyService.AddNewBlockItems(viewModel.BlockListName, listInfos).FirstOrDefault();
            }

            return View("~/ContentSafety/Views/BlockList/Index.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/AzureContentSafetyBlockList/GetBlockListItems")]
        public ActionResult GetBlockListItems(BlockListViewModel viewModel)
        {
            var model = new BlockListViewModel();
            model.ContentSafetyModel = new ContentSafetyModel();
            model.BlockLists = _optimizelyCmsHelper.GetTextBlockListsCMS();
            model.GetBlockItems = _optimizelyCmsHelper.GetBlockListItems();
            model.GetTextBlocklistsPages = _optimizelyCmsHelper.GetTextBlockLists();
            model.ContentSafetyModel.BlockItems = _azureAIContentSafetyService.GetBlockItems(viewModel.BlockListName);
            return View("~/ContentSafety/Views/BlockList/Index.cshtml", model);
        }
    }
}
