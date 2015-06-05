﻿using System;
using System.Fakes;
using System.Web;
using FakeItEasy;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeleniumStorageProvider.Enum;
using SeleniumStorageProvider.Interfaces;

namespace SeleniumStorageProvider.UnitTest
{
    [TestClass]
    public class StorageTest
    {
        
        
        private string Base64Image = @"
R0lGODlhCgAKAPYBAAAAAP///wAAAP///wAAAP///wAAAP///wAAAP///////wAAAP///wAAAP///wAAAP///wAAAP///wAAAAAAAP///wAAAP///wAAAP///wAAAP///wAAAP///////wAAAP///wAAAP///wAAAP///wAAAP///wAAAAAAAP///wAAAP///wAAAP///wAAAP///wAAAP///////wAAAP///wAAAP///wAAAP///wAAAP///wAAAAAAAP///wAAAP///wAAAP///wAAAP///wAAAP///////wAAAP///wAAAP///wAAAP///wAAAP///wAAAAAAAP///wAAAP///wAAAP///wAAAP///wAAAP///////wAAAP///wAAAP///wAAAP///wAAAP///wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACH5BAAAAAAALAAAAAAKAAoAAAdmgAABAgMEBQYHCAkKCwwNDg8QERITFBUWFxgZGhscHR4fICEiIyQlJicoKSorLC0uLzAxMjM0NTY3ODk6Ozw9Pj9AQUJDREVGR0hJSktMTU5PUFFSU1RVVldYWVpbXF1eX2BhYmOBADs=
";
        private string PageSource = "<html><head><title></title></head><body>Unit test</body></html>";
        private string Template = @"<!DOCTYPE html><!-- saved from url=(0115)https://seleniumscreenshots.blob.core.windows.net/seleniumscreenshots/EnablingFarmers/2015/3/20/error/12-53-24.html --><html lang="en" xmlns="http://www.w3.org/1999/xhtml"><head><meta http-equiv="Content-Type" content="text/html; charset=UTF-8"><meta charset="utf-8"><title></title><link rel="stylesheet" href="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.4/css/bootstrap.min.css"></head><body><style>body { padding-top: 20px; background-color: #eee; } .center { margin-left: auto; margin-right: auto; width: 1024px; } .center p { font-size: 20px; } .center p span { color: red; font-weight: bold; } pre { display: none; overflow: scroll; background-color: #fff; } pre.pageSource { margin: 20px; } img { max-width: 100%; cursor: pointer; } .modal-dialog { width: 100%; height: 100%; padding: 0; margin: 0; } .modal-content { height: 100%; border-radius: 0; color: white; overflow: auto; }</style><div class="container"><div class="well"><div class="row"><div class="col-md-8"><h1>Selenium test</h1><p><a target="_blank" href="https://github.com/marcoippel/Selenium-storage-provider" class="btn btn-info btn-block">https://github.com/marcoippel/Selenium-storage-provider</a></p><h2>Method</h2><p>can_save_html_template</p><h2>Message</h2><p>test if we can save the template</p></div><div class="col-md-4"><img data-toggle="modal" data-target="#myModal" src="data:image/png;base64,test" width="100%"></div></div></div><div class="well"><div class="row text-center"><a class="btn btn-primary pagesource">Click to show the page source:</a><pre class="pageSource text-left">&lt;html&gt;&lt;head&gt;&lt;title&gt;&lt;/title&gt;&lt;/head&gt;&lt;body&gt;Unit test&lt;/body&gt;&lt;/html&gt;</pre></div></div></div><!-- Modal --><div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true"><div class="modal-dialog modal-lg"><div class="modal-content"><div class="modal-header"><button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button><h4 class="modal-title" id="myModalLabel">Exception screenshot</h4></div><div class="modal-body"><img src="data:image/png;base64,test"></div><div class="modal-footer"><button type="button" class="btn btn-default" data-dismiss="modal">Close</button></div></div></div></div><!-- jQuery library --><script src="https://ajax.googleapis.com/ajax/libs/jquery/2.1.3/jquery.min.js"></script><!-- Latest compiled JavaScript --><script src="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.4/js/bootstrap.min.js"></script><script>$(document).ready(function () { $("a.pagesource").click(function () { $(".pageSource").slideToggle("slow"); }); });</script></body></html>";
        
        //  System.Text.Encoding.UTF8.GetString(byteArray);

        [TestInitialize]
        public void Setup()
        {
            
            
            
        }

        [TestMethod]
        public void Can_save_html_template()
        {
            using (ShimsContext.Create())
            {
                ShimDateTime.NowGet = () => new DateTime(2015, 05, 05, 10, 10, 10);

                IStorageProvider provider = A.Fake<IStorageProvider>();
                Storage storage = new Storage(provider);

                storage.Save("test", PageSource, "https://github.com/marcoippel/Selenium-storage-provider", "test if we can save the template", "can_save_html_template", EventType.Error);

                A.CallTo(() => provider.Save(null, "10-10-10.html", EventType.Error)).MustHaveHappened();
            }

            
        }
    }
}