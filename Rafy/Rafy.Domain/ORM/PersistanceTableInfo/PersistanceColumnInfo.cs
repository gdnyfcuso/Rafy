﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建日期：20150313
 * 运行环境：.NET 4.5
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20150313 17:27
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rafy.Domain.ORM
{
    class PersistanceColumnInfo : IPersistanceColumnInfo
    {
        private Type _dataType;
        private bool _isBooleanType;
        private bool _isStringType;

        public PersistanceTableInfo Table { get; set; }

        public string Name { get; set; }

        public Type DataType
        {
            get { return _dataType; }
            set
            {
                _dataType = value;
                _isBooleanType = _dataType == typeof(bool);
                _isStringType = _dataType == typeof(string);
            }
        }

        public bool IsBooleanType
        {
            get { return _isBooleanType; }
        }

        public bool IsStringType
        {
            get { return _isStringType; }
        }

        public bool IsIdentity { get; set; }

        public bool IsPrimaryKey { get; set; }

        public IProperty Property { get; set; }

        IPersistanceTableInfo IPersistanceColumnInfo.Table
        {
            get { return this.Table; }
        }
    }
}
