package com.example.server;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.Optional;

@Service
public class ArticleService {
    @Autowired
    private ArticleRepository articleRepository;

    private List<Article> articles = new ArrayList<>(Arrays.asList());

    public List<Article> getallArticles() {
        return (List<Article>) this.articleRepository.findAll();
    }

    public Optional<Article> getArticle(String id) {
        return this.articleRepository.findById(id);
    }

    public void addArticle(Article article) {
        this.articleRepository.save(article);
    }

    public void deleteArticle(String id) { this.articleRepository.deleteById(id); }

    public void updateArticle(String id, Article article) {
        this.articleRepository.save(article);
    }

    public Article getArticleById(String id) {
        for (Article a: articles) {
            if(a.getId()==id) {
                return a;
            }
        }
        return null;
    }
}
