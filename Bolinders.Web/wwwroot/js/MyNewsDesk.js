var result = [];
var articles = [];
var count = 0;
var hr;

$.getJSON(newsSettings.url + "?source=" + newsSettings.source + "&sortBy=" + newsSettings.sortBy + "&apiKey=" + newsSettings.apiKey, function (data) {
    result = data;

    $.each(result.articles, function (i, ob) {

        var shortText = jQuery.trim(ob.description).substring(0, 50)
            .split(" ").slice(0, -1).join(" ") + "...";
        ob.description = shortText;

        var dateObj = new Date(ob.publishedAt);
        var month = ('0' + (dateObj.getMonth() + 1)).slice(-2);
        var date = ('0' + dateObj.getDate()).slice(-2);
        var year = dateObj.getFullYear();
        var shortDate = year + '-' + month + '-' + date;
        ob.publishedAt = shortDate;

        articles.push(ob)
    });

    articles.sort(function (a, b) {
        var c = new Date(a.publishedAt);
        var d = new Date(b.publishedAt);
        return d - c;

    });

    if (articles.length > 3) {
        articles.length = 3;
    }

    $.each(articles, function (i, obj) {
        count += 1;
        var new_html = "<h5> <strong>" + obj.title + "</h5> </strong>"
            + "<p>" + obj.description + "</p>"
            + '<span class="small">' + obj.publishedAt + '</span>'
            + '<a class="pull-right target=" _blank href="' + obj.url + '">' + "Läs mer!" + '</a>'
            + (count < 3 ? '<hr />' : '')

        $('#nyheter').append($('<ul class="list-unstyled" style="-webkit-padding-start: 0px; list-style: none;"><li></li></ul>').html(new_html));
    });

    console.log(articles)
});
