import { GettingStartedViewModel } from '../../common/Web/directives/gettingstartedtemplate';

export class GettingStarted extends GettingStartedViewModel {
    constructor() {
        super();
        this.architectureDiagram = 'dist/Template/Microsoft-TwitterTemplate/Web/images/twitterArchitectureDiagram.png';
        this.features = [
            'Full cloud solution with minimum set up and maintenance considerations',
            'Real time data pulled from Twitter & enriched using machine learning',
            'Connect to Azure SQL and import data into Power BI'
        ];
        this.requirements = [
            'Azure Subscription',
            'Power BI Desktop (latest version)',
            'Power BI Pro (to share the template with your organization)',
            'Twitter Account'
        ];
        this.pricing = [
            'Processing 10K tweets a month will cost approximately $60',
            'Processing 50K tweets a month will cost approximately $165',
            'Processing 100K tweets a month will cost approximately $235',
        ];
        this.subTitle = 'Welcome to the public preview of the brand and campaign management solution template.';
        this.templateName = 'Brand and Campaign Management for Twitter';
    }
}