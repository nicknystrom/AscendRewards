
function formatPoints(points) {
    if (!points) return points;
    points = String(points);
    if (points.length < 4) return points;
    var i = points.length % 3;
    var result = [points.substr(0, i)];
    for (; i < points.length; i += 3) result.push(points.substr(i, 3));
    return result.join(',');
}


function ProductsPanel(panel, settings) {
    
    this.panel = panel;
    this.strip = $('<div class="product-strip" style="position: relative;"></div>');
    $(this.panel)
        .css({ position: 'relative', overflow: 'hidden' })
        .append(this.strip);

    this.config = {
        itemSize:    [180, 175],
        itemSpacing:         5,
        imageSize:   [140, 140],
        gridSize:        [3, 2]
    };
    if (settings) jQuery.extend(this.config, settings);

    this.template = $(this.config.template);
    if (!this.template)
        throw 'Must specify a template for a ProductsPanel.';

    this.products = this.config.products;
    if (this.products == undefined || !this.products)
        throw 'Must specify a product list when intializing the ProductsPanel.';
}

ProductsPanel.prototype.scroll = function (index) {
    var pageSize = this.config.gridSize[0] * this.config.gridSize[1];

    // normalize index so that we can't scroll farther than makes sense
    if (index >= this.products.length) index = this.products.length - (this.products.length % pageSize);
    if (index == this.products.length) index = this.products.length - pageSize;
    if (index < 0) index = 0;

    for (var i = index; i < index + (pageSize * 2) && i < this.products.length; i++) {
        var p = $('#product-' + i);
        if (p.length == 0) {
            this.strip.append(this.presentProduct(i));
        }
    }
    var left = this.productDisplayPosition(index)[0];
    if (index != this.strip.data('index')) {
        this.strip.animate({ left: -1 * left }, 'fast');
        this.strip.data('index', index);
    }

    if (this.config.scroll) {
        this.config.scroll(
            index,                          // first product shown
            this.config.products.length,    // total products
            pageSize,                       // page size
            Math.ceil(index / pageSize),    // page number
            Math.ceil(this.config.products.length / pageSize) // total pages
        );
    }

    return false;
};

ProductsPanel.prototype.page = function (pages) {
    var index = this.strip.data('index', index);
    index += pages * this.config.gridSize[0] * this.config.gridSize[1];
    return this.scroll(index);
};

ProductsPanel.prototype.presentProduct = function (index) {
    var p = this.products[index];
    var location = this.productDisplayPosition(index);
    var cell = $('#product-template > div').clone();
    var src = '';
    if (this.config.thumbnails) {
        src = this.config.thumbnails[index];
    }
    else {
        src = this.config.urlThumb.replace('(id)', p.Id)
                                  .replace('888888', this.config.imageSize[0])
                                  .replace('999999', this.config.imageSize[1]);
    }
    cell.attr('id', 'product-' + index);
    cell.attr('product', p.Id);
    cell.find('a').attr('href', this.config.urlProduct.replace('(id)', p.Id));
    cell.find('img')
        .attr('width', this.config.imageSize[0])
        .attr('height', this.config.imageSize[1])
        .attr('src', src);
    cell.find('.brand').text(p.Brand);
    cell.find('.name').text(p.Name);
    cell.find('.points').text(formatPoints(p.Price) + ' points');
    cell.css('left', location[0])
        .css('top', location[1]);
    return cell;
};

ProductsPanel.prototype.productDisplayPosition = function (index) {
    var column = Math.floor(index / this.config.gridSize[1]);
    var row = index % this.config.gridSize[1];
    return [
            (column * this.config.itemSize[0]) + (column * this.config.itemSpacing),
            (row * this.config.itemSize[1]) + (row * this.config.itemSpacing)
        ];
};

(function ($) {
    $.fn.products = function (settings, arg) {
        this.each(function () {
            if (typeof (settings) == 'string') {
                var widget = $(this).data('products');
                if (settings == 'scroll') return widget.scroll(arg);
                if (settings == 'page') return widget.page(arg);
            }
            else {
                var widget = new ProductsPanel(this, settings);
                $(this).data('products', widget);
                widget.scroll(0);
            }
        });
    }
})(jQuery);