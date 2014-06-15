﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OEA.Library;
using OEA.MetaModel;
using OEA.MetaModel.Attributes;
using OEA.MetaModel.View;
using OEA.Library.Validation;

namespace JXC
{
    [RootEntity, Serializable]
    public abstract class StorageInBill : JXCEntity
    {
        public static readonly RefProperty<Storage> StorageRefProperty =
            P<StorageInBill>.RegisterRef(e => e.Storage, ReferenceType.Normal);
        public int StorageId
        {
            get { return this.GetRefId(StorageRefProperty); }
            set { this.SetRefId(StorageRefProperty, value); }
        }
        public Storage Storage
        {
            get { return this.GetRefEntity(StorageRefProperty); }
            set { this.SetRefEntity(StorageRefProperty, value); }
        }

        public static readonly ListProperty<StorageInBillItemList> StorageInItemListProperty = P<StorageInBill>.RegisterList(e => e.StorageInItemList);
        public StorageInBillItemList StorageInItemList
        {
            get { return this.GetLazyList(StorageInItemListProperty); }
        }

        public static readonly Property<string> CodeProperty = P<StorageInBill>.Register(e => e.Code);
        public string Code
        {
            get { return this.GetProperty(CodeProperty); }
            set { this.SetProperty(CodeProperty, value); }
        }

        public static readonly Property<double> TotalMoneyProperty = P<StorageInBill>.Register(e => e.TotalMoney);
        public double TotalMoney
        {
            get { return this.GetProperty(TotalMoneyProperty); }
            set { this.SetProperty(TotalMoneyProperty, value); }
        }

        public static readonly Property<DateTime> DateProperty = P<StorageInBill>.Register(e => e.Date);
        public DateTime Date
        {
            get { return this.GetProperty(DateProperty); }
            set { this.SetProperty(DateProperty, value); }
        }

        public static readonly Property<string> CommentProperty = P<StorageInBill>.Register(e => e.Comment);
        public string Comment
        {
            get { return this.GetProperty(CommentProperty); }
            set { this.SetProperty(CommentProperty, value); }
        }

        protected override void AddValidations()
        {
            base.AddValidations();

            var rules = this.ValidationRules;
            rules.AddRule(CodeProperty, CommonRules.Required);
            rules.AddRule((e, args) =>
            {
                var po = e as StorageInBill;
                if (po.StorageInItemList.Count == 0)
                {
                    args.BrokenDescription = "至少需要一个商品项。";
                }
                else
                {
                    foreach (StorageInBillItem item in po.StorageInItemList)
                    {
                        if (item.View_TotalPrice <= 0)
                        {
                            args.BrokenDescription = "商品项金额应该是正数。";
                            return;
                        }
                    }
                }
            });
        }

        protected override void OnRoutedEvent(object sender, EntityRoutedEventArgs e)
        {
            if (e.Event == StorageInBillItem.PriceChangedEvent || e.Event == StorageInBillItemList.ListChangedEvent)
            {
                this.TotalMoney = this.StorageInItemList.Sum(poi => (poi as StorageInBillItem).View_TotalPrice);
            }
        }

        protected override void OnDelete()
        {
            base.OnDelete();

            //由于本类没有映射数据表，所以在删除的时候需要删除下面的数据
            using (var db = this.CreateDb())
            {
                db.Delete(typeof(StorageInBillItem), db.Query(typeof(StorageInBillItem))
                    .Constrain(StorageInBillItem.StorageInBillRefProperty).Equal(this.Id)
                    );
            }
        }

        public static readonly Property<string> StorageNameROProperty = P<StorageInBill>.RegisterReadOnly(
            e => e.StorageNameRO, e => (e as StorageInBill).GetStorageNameRO(), StorageRefProperty);
        /// <summary>
        /// 仓库名称（报表使用）
        /// </summary>
        public string StorageNameRO
        {
            get { return this.GetProperty(StorageNameROProperty); }
        }
        private string GetStorageNameRO()
        {
            return this.Storage.Name;
        }
    }

    [Serializable]
    public abstract class StorageInBillList : JXCEntityList
    {
        protected void QueryBy(TimeSpanCriteria criteria)
        {
            this.QueryDb(q =>
            {
                q.Constrain(StorageInBill.DateProperty).GreaterEqual(criteria.From)
                    .And().Constrain(StorageInBill.DateProperty).LessEqual(criteria.To);
            });
        }
    }

    public abstract class StorageInBillRepository : EntityRepository
    {
        protected StorageInBillRepository() { }
    }

    internal class StorageInBillConfig : EntityConfig<StorageInBill>
    {
        protected override void ConfigMeta()
        {
            Meta.MapTable().MapAllPropertiesToTable();
        }
    }
}