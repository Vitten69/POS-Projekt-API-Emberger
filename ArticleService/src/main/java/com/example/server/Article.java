package com.example.server;


import org.springframework.data.annotation.Id;

public class Article {
    @Id
    private String id;
    private String title;
    private String content;
    private String publisher;
    private String publishDate;
    private String category;

    public Article(){}

    public Article(String Id, String title, String content, String publisher, String publishDate, String category) {
        this.id = Id;
        this.title = title;
        this.content = content;
        this.publisher = publisher;
        this.publishDate = publishDate;
        this.category = category;
    }

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public String getContent() {
        return content;
    }

    public void setContent(String content) {
        this.content = content;
    }

    public String getPublisher() {
        return publisher;
    }

    public void setPublisher(String publisher) {
        this.publisher = publisher;
    }

    public String getPublishDate() {
        return publishDate;
    }

    public void setPublishDate(String publishDate) {
        this.publishDate = publishDate;
    }

    public String getCategory() {
        return category;
    }

    public void setCategory(String category) {
        this.category = category;
    }
}
