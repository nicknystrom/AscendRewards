using System;
using System.IO;
using System.Text;
using Net.SourceForge.Koogra.Excel.ValueTypes;
using System.Diagnostics;
using Net.SourceForge.Koogra.Text;

namespace Net.SourceForge.Koogra.Excel.Records
{
    /// <summary>
    /// This class abstracts a BOUNDSHEET (0x0085) Excel record found in Excel streams.
    /// </summary>
    public class BoundSheetRecord : Biff
    {
        private uint _bofPos;
        private VisibilityType _visibility;
        private SheetType _type;
        private string _name;

        /// <summary>
        /// The constructor for the record.
        /// </summary>
        /// <param name="biff">The GenericBiff record that should contain the correct type and data for the BOUNDSHEET record.</param>
        /// <exception cref="InvalidRecordIdException">
        /// An InvalidRecordIdException is thrown if biff contains an invalid type or invalid data.
        /// </exception>
        public BoundSheetRecord(GenericBiff biff)
        {
            if (biff.Id == (ushort)RecordType.Boundsheet)
            {
                BinaryReader reader = new BinaryReader(biff.GetDataStream());
                _bofPos = reader.ReadUInt32();
                _visibility = (VisibilityType)reader.ReadByte();
                _type = (SheetType)reader.ReadByte();

                byte len = reader.ReadByte();
                _name = Reader.ReadPossibleCompressedString(reader, len);
            }
            else
                throw new InvalidRecordIdException(biff.Id, RecordType.Boundsheet);
        }

        /// <summary>
        /// The absolute position in the stream of the BOF record associated with this BOUNDSHEET record.
        /// </summary>
        public uint BofPos
        {
            get
            {
                return _bofPos;
            }
        }

        /// <summary>
        /// The visibility of this BOUNDSHEET record.
        /// </summary>
        public VisibilityType Visibility
        {
            get
            {
                return _visibility;
            }
        }

        /// <summary>
        /// The type for the BOUNDSHEET record.
        /// </summary>
        public SheetType Type
        {
            get
            {
                return _type;
            }
        }

        /// <summary>
        /// The name of the BOUNDSHEET record.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }
    }
}
