import { DataStoreType } from '../../../../../SiteCommon/Web/enums/data-store-type';

import { NewsEntity } from '../../../../../SiteCommon/Web/models/news-entity';

import { ViewModelBase } from '../../../../../SiteCommon/Web/services/view-model-base';

export class Customize extends ViewModelBase {
    csv: FileList = null;
    csvPath: string = null;
    delimiter: string = '\r\n';
    entities: NewsEntity[] = [];
    icons: string[] = ['glass', 'music', 'search', 'envelope-o', 'heart', 'star', 'star-o', 'user', 'film', 'th-large', 'th', 'th-list', 'check', 'times', 'search-plus', 'search-minus', 'power-off', 'signal', 'cog', 'trash-o', 'home', 'file-o', 'clock-o', 'road', 'download', 'arrow-circle-o-down', 'arrow-circle-o-up', 'inbox', 'play-circle-o', 'repeat', 'refresh', 'list-alt', 'lock', 'flag', 'headphones', 'volume-off', 'volume-down', 'volume-up', 'qrcode', 'barcode', 'tag', 'tags', 'book', 'bookmark', 'print', 'camera', 'font', 'bold', 'italic', 'text-height', 'text-width', 'align-left', 'align-center', 'align-right', 'align-justify', 'list', 'outdent', 'indent', 'video-camera', 'picture-o', 'pencil', 'map-marker', 'adjust', 'tint', 'pencil-square-o', 'share-square-o', 'check-square-o', 'arrows', 'step-backward', 'fast-backward', 'backward', 'play', 'pause', 'stop', 'forward', 'fast-forward', 'step-forward', 'eject', 'chevron-left', 'chevron-right', 'plus-circle', 'minus-circle', 'times-circle', 'check-circle', 'question-circle', 'info-circle', 'crosshairs', 'times-circle-o', 'check-circle-o', 'ban', 'arrow-left', 'arrow-right', 'arrow-up', 'arrow-down', 'share', 'expand', 'compress', 'plus', 'minus', 'asterisk', 'exclamation-circle', 'gift', 'leaf', 'fire', 'eye', 'eye-slash', 'exclamation-triangle', 'plane', 'calendar', 'random', 'comment', 'magnet', 'chevron-up', 'chevron-down', 'retweet', 'shopping-cart', 'folder', 'folder-open', 'arrows-v', 'arrows-h', 'bar-chart', 'twitter-square', 'facebook-square', 'camera-retro', 'key', 'cogs', 'comments', 'thumbs-o-up', 'thumbs-o-down', 'star-half', 'heart-o', 'sign-out', 'linkedin-square', 'thumb-tack', 'external-link', 'sign-in', 'trophy', 'github-square', 'upload', 'lemon-o', 'phone', 'square-o', 'bookmark-o', 'phone-square', 'twitter', 'facebook', 'github', 'unlock', 'credit-card', 'rss', 'hdd-o', 'bullhorn', 'bell', 'certificate', 'hand-o-right', 'hand-o-left', 'hand-o-up', 'hand-o-down', 'arrow-circle-left', 'arrow-circle-right', 'arrow-circle-up', 'arrow-circle-down', 'globe', 'wrench', 'tasks', 'filter', 'briefcase', 'arrows-alt', 'users', 'link', 'cloud', 'flask', 'scissors', 'files-o', 'paperclip', 'floppy-o', 'square', 'bars', 'list-ul', 'list-ol', 'strikethrough', 'underline', 'table', 'magic', 'truck', 'pinterest', 'pinterest-square', 'google-plus-square', 'google-plus', 'money', 'caret-down', 'caret-up', 'caret-left', 'caret-right', 'columns', 'sort', 'sort-desc', 'sort-asc', 'envelope', 'linkedin', 'undo', 'gavel', 'tachometer', 'comment-o', 'comments-o', 'bolt', 'sitemap', 'umbrella', 'clipboard', 'lightbulb-o', 'exchange', 'cloud-download', 'cloud-upload', 'user-md', 'stethoscope', 'suitcase', 'bell-o', 'coffee', 'cutlery', 'file-text-o', 'building-o', 'hospital-o', 'ambulance', 'medkit', 'fighter-jet', 'beer', 'h-square', 'plus-square', 'angle-double-left', 'angle-double-right', 'angle-double-up', 'angle-double-down', 'angle-left', 'angle-right', 'angle-up', 'angle-down', 'desktop', 'laptop', 'tablet', 'mobile', 'circle-o', 'quote-left', 'quote-right', 'spinner', 'circle', 'reply', 'github-alt', 'folder-o', 'folder-open-o', 'smile-o', 'frown-o', 'meh-o', 'gamepad', 'keyboard-o', 'flag-o', 'flag-checkered', 'terminal', 'code', 'reply-all', 'star-half-o', 'location-arrow', 'crop', 'code-fork', 'chain-broken', 'question', 'info', 'exclamation', 'superscript', 'subscript', 'eraser', 'puzzle-piece', 'microphone', 'microphone-slash', 'shield', 'calendar-o', 'fire-extinguisher', 'rocket', 'maxcdn', 'chevron-circle-left', 'chevron-circle-right', 'chevron-circle-up', 'chevron-circle-down', 'html5', 'css3', 'anchor', 'unlock-alt', 'bullseye', 'ellipsis-h', 'ellipsis-v', 'rss-square', 'play-circle', 'ticket', 'minus-square'];
    selectedEntity: NewsEntity = null;



    entityUpload(): void {
        this.MS.UtilityService.readFile(this.csv[0], (content: any) => {
            let data: string[][] = this.MS.UtilityService.parseCsv(content);
            let entities: NewsEntity[] = [];

            let headers: string[] = data[0];

            for (let i = 0; i < headers.length; i++) {
                let entity: NewsEntity = new NewsEntity();
                entity.name = headers[i];
                entity.values = '';
                entities.push(entity);
            }

            for (let i = 1; i < data.length; i++) {
                let row: string[] = data[i];
                for (let j = 0; j < row.length; j++) {
                    if (row[j]) {
                        entities[j].values += row[j] + this.delimiter;
                    }
                }
            }

            this.entities = this.entities.concat(entities);
            this.selectedEntity = this.entities[this.entities.length - 1];

            this.csvPath = null;
        });
    }

    iconSelect(icon: string): void {
        this.selectedEntity.icon = icon;
    }

    async onLoaded(): Promise<void> {
        this.isValidated = true;

        let entities: NewsEntity[] = JSON.parse(this.MS.DataStore.getValue('UserDefinedEntities'));
        this.entities = entities || [];
    }

    async onNavigatingNext(): Promise<boolean> {
        let validator: any = {};

        for (let i = 0; i < this.entities.length; i++) {
            let entity: NewsEntity = this.entities[i];

            entity.name = entity.name.trim() || this.MS.Translate.COMMON_BLANK;
            entity.values = entity.values.trim();

            if (validator[entity.name]) {
                validator[entity.name].values += this.delimiter + entity.values;
            } else {
                validator[entity.name] = entity;
            }
        }

        let validEntities: NewsEntity[] = [];
        for (let key in validator) {
            if (validator.hasOwnProperty(key)) {
                validEntities.push(validator[key]);
            }
        }

        this.MS.DataStore.addToDataStore('UserDefinedEntities', JSON.stringify(validEntities), DataStoreType.Public);

        return true;
    }
}