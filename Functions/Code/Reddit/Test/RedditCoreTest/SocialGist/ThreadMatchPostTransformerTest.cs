using RedditCore.SocialGist;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedditCore.SocialGist.Model;
using System.Collections.Generic;

namespace RedditCoreTest.SocialGist
{
    [TestClass]
    public class ThreadMatchPostTransformerTest
    {

        [TestMethod]
        public void GetMediaPreviewUrl_nulls_and_empties()
        {
            Assert.IsNull(ThreadResponsePostTransformer.GetMediaPreviewUrl(null), "Null thread object");
            var thread = new ResponsePost();
            Assert.IsNull(ThreadResponsePostTransformer.GetMediaPreviewUrl(thread), "Empty thread object with null properties");

            // thread.media.oembed.thumbnail_url path
            thread.Media = new Media();
            Assert.IsNull(ThreadResponsePostTransformer.GetMediaPreviewUrl(thread), "Thread object with only media property set, but media property is set to empty media object");
            thread.Media.Oembed = new Oembed();
            Assert.IsNull(ThreadResponsePostTransformer.GetMediaPreviewUrl(thread), "Thread object with only media property set and oembed property set to empty Oembed2 object (no url set)");

            thread = new ResponsePost();
            // thread.preview.images[0].source.url path
            thread.Preview = new Preview();
            Assert.IsNull(ThreadResponsePostTransformer.GetMediaPreviewUrl(thread), "Thread object with only preview property set, but preview property is set to empty preview object");
            thread.Preview.Images = new List<Image>();
            Assert.IsNull(ThreadResponsePostTransformer.GetMediaPreviewUrl(thread), "Thread object with only preview property set and images property set to empty list of Images");
            thread.Preview.Images.Add(null);
            Assert.IsNull(ThreadResponsePostTransformer.GetMediaPreviewUrl(thread), "Thread object with only preview property set and images property set with a single null element added to list of Images");
            thread.Preview.Images[0] = new Image();
            Assert.IsNull(ThreadResponsePostTransformer.GetMediaPreviewUrl(thread), "Thread object with only preview property set and images property set with a single empty image element added to list");
            thread.Preview.Images[0].Source = new Source();
            Assert.IsNull(ThreadResponsePostTransformer.GetMediaPreviewUrl(thread), "Thread object with only preview property set and images property set with a single image element added with an empty source property");

            thread = new ResponsePost()
            {
                Media = new Media()
                {
                    Oembed = new Oembed()
                },
                Preview = new Preview()
                {
                    Images = new List<Image>()
                    {
                        new Image()
                        {
                            Source = new Source()
                        }
                    }
                }
            };
            Assert.IsNull(ThreadResponsePostTransformer.GetMediaPreviewUrl(thread), "All items but urls set; must still return null");
        }

        [TestMethod]
        public void GetMediaPreviewUrl_no_preview()
        {
            var thread = new ResponsePost()
            {
                Media = new Media()
                {
                    Oembed = new Oembed()
                    {
                        ThumbnailUrl = "http://foo.com",
                        ThumbnailHeight = 2,
                        ThumbnailWidth = 2
                    }
                }
            };

            Assert.AreEqual("http://foo.com", ThreadResponsePostTransformer.GetMediaPreviewUrl(thread));
        }

        [TestMethod]
        public void GetMediaPreviewUrl_no_media()
        {
            var thread = new ResponsePost()
            {
                Preview = new Preview()
                {
                    Images = new List<Image>()
                    {
                        new Image()
                        {
                            Source = new Source()
                            {
                                Url = "http://foo.com",
                                Height = 2,
                                Width = 2
                            }
                        }
                    }
                }
            };

            Assert.AreEqual("http://foo.com", ThreadResponsePostTransformer.GetMediaPreviewUrl(thread));

        }

        [TestMethod]
        public void GetMediaPreviewUrl_smaller_preview()
        {
            var thread = new ResponsePost()
            {

                Media = new Media()
                {
                    Oembed = new Oembed()
                    {
                        ThumbnailUrl = "http://foo.com",
                        ThumbnailHeight = 3,
                        ThumbnailWidth = 3
                    }
                },
                Preview = new Preview()
                {
                    Images = new List<Image>()
                    {
                        new Image()
                        {
                            Source = new Source()
                            {
                                Url = "http://bar.com",
                                Height = 2,
                                Width = 2
                            }
                        }
                    }
                }
            };
            Assert.AreEqual("http://bar.com", ThreadResponsePostTransformer.GetMediaPreviewUrl(thread));
        }


        [TestMethod]
        public void GetMediaPreviewUrl_smaller_media()
        {
            var thread = new ResponsePost()
            {

                Media = new Media()
                {
                    Oembed = new Oembed()
                    {
                        ThumbnailUrl = "http://foo.com",
                        ThumbnailHeight = 3,
                        ThumbnailWidth = 3
                    }
                },
                Preview = new Preview()
                {
                    Images = new List<Image>()
                    {
                        new Image()
                        {
                            Source = new Source()
                            {
                                Url = "http://bar.com",
                                Height = 4,
                                Width = 4
                            }
                        }
                    }
                }
            };
            Assert.AreEqual("http://foo.com", ThreadResponsePostTransformer.GetMediaPreviewUrl(thread));
        }
    }
}
