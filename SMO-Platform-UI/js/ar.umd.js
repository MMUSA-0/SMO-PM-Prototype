! function (o, e) {
    "object" == typeof exports && "undefined" != typeof module ? e(exports) : "function" == typeof define && define.amd ? define(["exports"], e) : e(((o = "undefined" != typeof globalThis ? globalThis : o || self).Fancybox = o.Fancybox || {}, o.Fancybox.l10n = o.Fancybox.l10n || {}))
}(this, (function (o) {
    "use strict";
    const e = Object.assign(Object.assign({}, {
        PANUP: "تحريك لأعلى",
        PANDOWN: "تحريك لأسفل",
        PANLEFT: "تحريك لليسار",
        PANRIGHT: "تحريك لليمين",
        ZOOMIN: "تكبير الصورة",
        ZOOMOUT: "تصغير الصورة",
        TOGGLEZOOM: "تبديل مستوى التكبير",
        TOGGLE1TO1: "تبديل مستوى التكبير",
        ITERATEZOOM: "تبديل مستوى التكبير",
        ROTATECCW: "تدوير عكس اتجاه عقارب الساعة",
        ROTATECW: "تدوير في اتجاه عقارب الساعة",
        FLIPX: "انعكاس أفقياً",
        FLIPY: "تقلب عمودياً",
        FITX: "تناسب أفقياً",
        FITY: "تناسب عمودياً",
        RESET: "إعادة ضبط",
        TOGGLEFS: "تبديل ملء الشاشة"
    }), {
        CLOSE: "إغلاق",
        NEXT: "التالي",
        PREV: "السابق",
        MODAL: "يمكنك إغلاق محتوى الشاشة هذه باستخدام مفتاح ESC",
        ERROR: "حدث خطأ ما، يرجى المحاولة مرة أخرى لاحقاً",
        IMAGE_ERROR: "لم يتم العثور على الصورة",
        ELEMENT_NOT_FOUND: "لم يتم العثور على عنصر HTML",
        AJAX_NOT_FOUND: "خطأ في تحميل AJAX: غير موجود",
        AJAX_FORBIDDEN: "خطأ في تحميل AJAX: محظور",
        IFRAME_ERROR: "خطأ في تحميل الصفحة",
        TOGGLE_ZOOM: "تبديل مستوى التكبير",
        TOGGLE_THUMBS: "تبديل وضع الصور المصغرة",
        TOGGLE_SLIDESHOW: "التبديل بين عرض الشرائح",
        TOGGLE_FULLSCREEN: "تبديل وضع ملء الشاشة",
        DOWNLOAD: "تحميل"
    });
    o.ar = e
}));