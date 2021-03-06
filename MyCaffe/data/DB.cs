﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyCaffe.db.image;
using MyCaffe.basecode;
using MyCaffe.basecode.descriptors;

namespace MyCaffe.data
{
    /// <summary>
    /// A generic database class used to connect to the underlying database and create a Cursor that traverses through it.
    /// </summary>
    public class DB
    {
        IXImageDatabase m_db = null;
        int m_nSrcID = 0;
        string m_strSrc = null;

        /// <summary>
        /// The DB Constructor.
        /// </summary>
        /// <param name="imgDb">The underlying database - the CaffeImageDatabase.</param>
        public DB(IXImageDatabase imgDb)
        {
            m_db = imgDb;
        }

        /// <summary>
        /// Opens the underlying database with a given data source.
        /// </summary>
        /// <param name="strSrc">Specifies the data source name.</param>
        public void Open(string strSrc)
        {
            m_strSrc = strSrc;
            m_nSrcID = m_db.GetSourceID(strSrc);
        }

        /// <summary>
        /// Closes the last Open session.
        /// </summary>
        public void Close()
        {
            m_nSrcID = 0;
        }

        /// <summary>
        /// Creates and returns a new Cursor used to traverse through a data source within the database.
        /// </summary>
        /// <returns></returns>
        public Cursor NewCursor()
        {
            return new Cursor(m_db, m_strSrc);
        }
    }

    /// <summary>
    /// The Cursor is used to traverse through a given data source within the database.
    /// </summary>
    public class Cursor 
    {
        IXImageDatabase m_db;
        int m_nSrcID = 0;
        int m_nCount = 0;
        int m_nIdx = 0;

        /// <summary>
        /// The Cursor constructor.
        /// </summary>
        /// <param name="db">Specifies the underlying database.</param>
        /// <param name="strSrc">Specifies the name of the data source to use.</param>
        public Cursor(IXImageDatabase db, string strSrc)
        {
            m_db = db;
            SourceDescriptor src = m_db.GetSourceByName(strSrc);
            m_nSrcID = src.ID;
            m_nCount = src.ImageCount;
        }

        /// <summary>
        /// Resets the current index bact to the start.
        /// </summary>
        public void Reset()
        {
            m_nIdx = 0;
        }

        /// <summary>
        /// Traverses to the next item within the data source.
        /// </summary>
        public void Next()
        {
            m_nIdx++;
        }

        /// <summary>
        /// Queryies to see if we are still within the bounds of the data source, if so <i>true</i> is returned, 
        /// otherwise if the Cursor it as the end of the data source, <i>false</i> is returned.
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (m_nIdx >= m_nCount)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Move the cursor to the beginning of the data source.
        /// </summary>
        public void SeekToFirst()
        {
            m_nIdx = 0;
        }

        /// <summary>
        /// Retrieve the Datum at the current cursor location within the data source.
        /// </summary>
        /// <param name="nLabel">Optionally, specifies a label for which the cursor should query from.</param>
        /// <returns>The Datum retrieved is returned.</returns>
        public Datum GetValue(int? nLabel = null)
        {
            SimpleDatum sd = m_db.QueryImage(m_nSrcID, m_nIdx, null, null, nLabel);
            return new Datum(sd);
        }
    }
}
