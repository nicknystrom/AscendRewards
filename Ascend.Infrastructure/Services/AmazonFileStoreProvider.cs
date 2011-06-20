using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascend.Core.Services;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;

namespace Ascend.Infrastructure.Services
{
    public class AmazonFileStoreProvider : IFileStoreProvider
    {
        public IInfrastructureConfiguration Configuration { get; set; }
        public ILog Log { get; set; }

        private AmazonS3Client _s3;
        private Dictionary<string, __Store> _stores;

        private AmazonS3Client S3
        {
            get
            {
                if (null == _s3)
                {
                    lock (this)
                    {
                        if (null == _s3)
                        {
                            _s3 = new AmazonS3Client(
                                Configuration.AmazonAccessKey,
                                Configuration.AmazonSecretKey,
                                new AmazonS3Config
                                {
                                    UseSecureStringForAwsSecretKey = false,
                                    CommunicationProtocol = Protocol.HTTP,
                                }
                            );
                        }
                    }
                }
                return _s3;
            }
        }

        class __Store : IFileStore
        {
            IDictionary<string, string> _keys;
            AmazonS3Client _s3;
            string _bucket;

            public __Store(AmazonS3Client s3, string bucket)
            {
                _s3 = s3;
                _bucket = bucket;
            }

            public string Name
            {
                get { return _bucket; }
            }

            void Init()
            {
                if (null == _keys)
                {
                    lock (this)
                    {
                        if (null == _keys)
                        {
                            _keys = new Dictionary<string, string>();
                            var objects = _s3.ListObjects(new ListObjectsRequest().WithBucketName(_bucket));
                            while (true)
                            {
                                // add all image names from this batch
                                foreach (var o in objects.S3Objects)
                                {
                                    _keys.Add(o.Key, o.Key);
                                }

                                // retrieve the next batchs
                                if (!String.IsNullOrEmpty(objects.NextMarker))
                                {
                                    objects = _s3.ListObjects(new ListObjectsRequest().WithBucketName(_bucket).WithMarker(objects.NextMarker));
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            public bool Contains(string name)
            {
                lock (this)
                {
                    Init();
                    return _keys.ContainsKey(name);
                }
            }

            public ICollection<string> List()
            {
                lock (this)
                {
                    Init();
                    return _keys.Keys.ToArray();
                }
            }

            public System.Tuple<byte[], string> Get(string name)
            {
                var obj = _s3.GetObject(
                    new GetObjectRequest()
                    .WithBucketName(Name)
                    .WithKey(name));
                return System.Tuple.Create(
                    obj.ResponseStream.ToArray(obj.ContentLength),
                    obj.ContentType);
            }

            public void Put(string name, byte[] contents, string type)
            {
                lock (this)
                {
                    Init();
                    if (!_keys.ContainsKey(name))
                    {
                        _keys.Add(name, name);
                    }
                }
                var request = (PutObjectRequest)
                    new PutObjectRequest()
                    .WithBucketName(_bucket)
                    .WithContentType(type)
                    .WithKey(name)
                    .WithStorageClass(S3StorageClass.ReducedRedundancy)
                    .WithCannedACL(S3CannedACL.PublicRead)
                    .WithInputStream(new MemoryStream(contents));
                try
                {
                    _s3.PutObject(request);
                }
                catch
                {
                    try
                    {
                        System.Threading.Thread.Sleep(500);
                        _s3.PutObject(request);
                    }
                    catch
                    {
                        // not exactly atomic, but remove the name if the upload failed
                        lock (this)
                        {
                            if (_keys.ContainsKey(name))
                            {
                                _keys.Remove(name);
                            }
                        }
                    }
                }
            }

            public void Delete(string name)
            {
                lock (this)
                {
                    Init();
                    if (!_keys.ContainsKey(name))
                    {
                        return;
                    }
                    _keys.Remove(name);
                }
                try
                {
                    _s3.DeleteObject(
                        new DeleteObjectRequest()
                        .WithBucketName(_bucket)
                        .WithKey(name)
                    );
                }
                catch
                {
                    lock (this)
                    {
                        if (!_keys.ContainsKey(name))
                        {
                            _keys.Add(name, name);
                        }
                    }
                }
            }

            public string GetUrl(string name)
            {
                return String.Format("https://s3.amazonaws.com/{0}/{1}", _bucket, name);
            }
        }

        public string[] ListFileStores()
        {
            return S3.ListBuckets().Buckets.Select(x => x.BucketName).ToArray();
        }

        public IFileStore GetFileStore(string name, bool create = true)
        {
            lock (this)
            {
                if (null == _stores)
                {
                    _stores = ListFileStores().ToDictionary(x => x, x => new __Store(S3, x));
                }
                if (!_stores.ContainsKey(name))
                {
                    if (create)
                    {
                        S3.PutBucket(
                            new PutBucketRequest()
                            .WithBucketName(name)
                            .WithBucketRegion(S3Region.US)
                        );
                        _stores[name] = new __Store(S3, name);
                    }
                    else
                    {
                        throw new Exception("Bucket does not exist and 'create' is false.");
                    }
                }
                return _stores[name];
            }
        }
    }


}
