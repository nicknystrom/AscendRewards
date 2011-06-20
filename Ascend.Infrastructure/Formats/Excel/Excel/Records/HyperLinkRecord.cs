using System;
using System.IO;
using Net.SourceForge.Koogra.Text;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// This class abstract an HLINK (0x01B8) record found in Excel streams.
	/// Thanks to openoffice.org as the format was taken from their documentation of the excel file format http://www.openoffice.org
	/// </summary>
	public class HyperLinkRecord : Biff
	{
		private static readonly byte[] UrlMonikerGuid = new byte[] { 0xE0, 0xC9, 0xEA, 0x79, 0xF9, 0xBA, 0xCE, 0x11, 0x8C, 0x82, 0x00, 0xAA, 0x00, 0x4B, 0xA9, 0x0B, };
		private static readonly byte[] FileMonikerGuid = new byte[] { 0x03, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46, };

		private ushort _firstRow;
		private ushort _lastRow;
		private ushort _firstCol;
		private ushort _lastCol;
		private byte[] _guid;		
		private HyperLinkOptions _options;
		private byte[] _monikerGuid;
		private string _link = "";
		private string _description = "";
		private string _textMark = "";
		private string _targetFrame = "";

		private static bool CompareArrays(byte[] l, byte[] r)
		{
			if(l.Length != r.Length)
				return false;

			for(int n = 0; n < l.Length; ++n)
			{
				if(l[n] != r[n])
					return false;
			}

			return true;
		}

		private bool IsUrlMoniker(byte[] moniker)
		{
			return CompareArrays(moniker, HyperLinkRecord.UrlMonikerGuid);
		}

		private bool IsFileMoniker(byte[] moniker)
		{
			return CompareArrays(moniker, HyperLinkRecord.FileMonikerGuid);
		}

		/// <summary>
		/// The constructor for the record.
		/// </summary>
		/// <param name="biff">The GenericBiff record that should contain the correct type and data for the HLINK record.</param>
		/// <exception cref="InvalidRecordIdException">
		/// An InvalidRecordIdException is thrown if biff contains an invalid type or invalid data.
		/// </exception>
		public HyperLinkRecord(GenericBiff biff)
		{
			if(biff.Id == (ushort)RecordType.Hyperlink)
			{
				BinaryReader reader = new BinaryReader(biff.GetDataStream());

				_firstRow = reader.ReadUInt16();
				_lastRow = reader.ReadUInt16();
				_firstCol = reader.ReadUInt16();
				_lastCol = reader.ReadUInt16();

				_guid = reader.ReadBytes(16);

				/* uint unknown = */ reader.ReadUInt32();

				_options = new HyperLinkOptions(reader.ReadUInt32());

				if(_options.HasDescription)
				{
					_description = Reader.ReadSimpleUnicodeString(reader);
				}

				if(_options.HasTargetFrame)
				{
					_targetFrame = Reader.ReadSimpleUnicodeString(reader);
				}

				if(_options.HasFileLinkOrUrl)
				{
					_monikerGuid = reader.ReadBytes(16);

					if(IsUrlMoniker(_monikerGuid))
					{
						_link = Reader.ReadSimpleUnicodeString(reader);
					}
					else if(IsFileMoniker(_monikerGuid))
					{
						if(_options.HasUNCPath)
						{
							_link = Reader.ReadSimpleUnicodeString(reader);
						}
						else
						{
							/* int uplevelCount = */ reader.ReadUInt16();

							int shortFileLen = reader.ReadInt32();

							string shortFile = Reader.ReadSimpleAsciiString(reader, shortFileLen);

							/* byte[] unknownBytes = */ reader.ReadBytes(24);

							int fileLinkLen = reader.ReadInt32();

							if(fileLinkLen > 0)
							{
								int extendedFileLen = reader.ReadInt32();

								/* byte[] unknownBytes2 = */ reader.ReadBytes(2);

								string extendedFile = Reader.ReadSimpleUnicodeString(reader, extendedFileLen);

								_link = extendedFile;
							}
							else
								_link = shortFile;
						}
					}
				}

				if(_options.HasTextMark)
				{
                    try
                    {
                        _textMark = Reader.ReadSimpleUnicodeString(reader);
                    }
                    catch
                    {
                    }
				}
			}
			else
				throw new InvalidRecordIdException(biff.Id, RecordType.Hyperlink);
		}

		/// <summary>
		/// The first row where the hyperlink applies to.
		/// </summary>
		public ushort FirstRow
		{
			get
			{
				return _firstRow;
			}
		}

		/// <summary>
		/// The last row where the hyperlink applies to.
		/// </summary>
		public ushort LastRow
		{
			get
			{
				return _lastRow;
			}
		}

		/// <summary>
		/// The first column where the hyperlink applies to.
		/// </summary>
		public ushort FirstCol
		{
			get
			{
				return _firstCol;
			}
		}

		/// <summary>
		/// The last column where the hyperlink applies to.
		/// </summary>
		public ushort LastCol
		{
			get
			{
				return _lastCol;
			}
		}

		/// <summary>
		/// The link GUID.
		/// </summary>
		public byte[] Guid
		{
			get
			{
				return _guid;
			}
		}

		/// <summary>
		/// The hyperlink options flag.
		/// </summary>
		public HyperLinkOptions Options
		{
			get
			{
				return _options;
			}
		}

		/// <summary>
		/// The moniker GUID of the link. Used to identify if the link is a URL or file link.
		/// </summary>
		public byte[] MonikerGuid
		{
			get
			{
				return _monikerGuid;
			}
		}

		/// <summary>
		/// The link.
		/// </summary>
		public string Link 
		{
			get
			{
				return _link;
			}
		}

		/// <summary>
		/// Description of the link.
		/// </summary>
		public string Description 
		{
			get
			{
				return _description;
			}
		}

		/// <summary>
		/// Text mark.
		/// </summary>
		public string TextMark 
		{
			get
			{
				return _textMark;
			}
		}

		/// <summary>
		/// Link target frame.
		/// </summary>
		public string TargetFrame 
		{
			get
			{
				return _targetFrame;
			}
		}

	}
}
